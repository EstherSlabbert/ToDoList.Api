using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Data;
using ToDoList.DTOs;
using ToDoList.Entities;
using System.Net;
using System.Net.Http.Json;
using ToDoList.Common;

namespace AcceptanceTests
{
    public class ToDoListApiTests : IClassFixture<ToDoListWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly DataContext _dbContext;
        private int? _testToDoId;
        private Project? _existingProject;

        public ToDoListApiTests(ToDoListWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            // Resolve the database context for cleanup
            var scope = factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        }

        // Runs before each test to ensure a clean database
        public async Task InitializeAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ToDos");
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Projects");
        }

        private async Task AddDefaultProjectToDatabase()
        {
            _existingProject = new Project("Existing Test Project Name", "Existing Test Project Description");
            await _dbContext.Projects.AddAsync(_existingProject);
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddDefaultToDoToDatabase()
        {
            // Ensure the project exists before adding the ToDo
            await AddDefaultProjectToDatabase();
            // Add default test ToDo
            var testToDo = new ToDo
            {
                Title = "Test",
                Description = "Test",
                DueDate = new DateTime(2025, 01, 01, 12, 00, 00),
                Project = _existingProject, // Ensure this project is valid
                Priority = 1,
                IsCompleted = false
            };
            await _dbContext.ToDos.AddAsync(testToDo);
            await _dbContext.SaveChangesAsync();

            // Store the generated ID for test assertions
            _testToDoId = testToDo.Id;
        }

        // Runs after each test (optional, for additional cleanup)
        public async Task DisposeAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ToDos");
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Projects");
        }

        //* ToDoApi *//

        [Fact]
        public async Task GetAllToDos_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultToDoToDatabase();

            // Act
            var response = await _client.GetAsync("/api/ToDo");

            // Assert
            response.EnsureSuccessStatusCode();
            var heroes = await response.Content.ReadFromJsonAsync<List<ToDoDetailsDto>>();
            heroes.ShouldNotBeNull();
            heroes.ShouldNotBeEmpty();
            heroes.ShouldBeOfType<List<ToDoDetailsDto>>();
            heroes.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task CreateToDo_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultProjectToDatabase();
            var newToDo = new ToDoDto("New Test ToDo", "New Test Description", new DateTime(2025, 01, 01, 12, 00, 00), 2, _existingProject.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/ToDo", newToDo);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateToDo_ReturnsErrorWhenTitleInputIsInvalid()
        {
            // Arrange & Act
            await AddDefaultProjectToDatabase();
            var act = () => _client.PostAsJsonAsync("/api/ToDo", new ToDoDto(null, "", new DateTime(2025, 01, 01, 12, 00, 00), 2, _existingProject.Id));

            // Assert
            act.ShouldThrow<InvalidParameterException>($@"The argument ""title"" was not provided.");
        }

        [Fact]
        public async Task CreateToDo_ReturnsErrorWhenProjectDoesNotExist()
        {
            // Arrange & Act
            var response = await _client.PostAsJsonAsync("/api/ToDo", new ToDoDto("Something", "", new DateTime(2025, 01, 01, 12, 00, 00), 2, 100));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldContain("\"title\":\"Not Found\"");
            content.ShouldContain("\"status\":404");
            content.ShouldContain("\"detail\":\"Project with Id \\\"100\\\" was not found.\"");
        }

        [Fact]
        public async Task CreateToDo_StoresToDoInDatabase()
        {
            // Arrange
            await AddDefaultProjectToDatabase();
            var newToDo = new ToDoDto("New Test ToDo 1", "New Test Description 1", new DateTime(2025, 01, 01, 12, 00, 00), 2, _existingProject.Id);

            // Act
            var response = await _client.PostAsJsonAsync("/api/ToDo", newToDo);
            response.EnsureSuccessStatusCode();

            // Assert
            var createdToDo = await _dbContext.ToDos.AsNoTracking()
                .Include(x => x.Project)
                .FirstOrDefaultAsync(h => h.Title == "New Test ToDo 1");

            createdToDo.ShouldNotBeNull();
            createdToDo.Title.ShouldBe(newToDo.Title);
            createdToDo.Description.ShouldBe(newToDo.Description);
            createdToDo.DueDate.ShouldBe(newToDo.DueDate);
            createdToDo.Priority.ShouldBe(newToDo.Priority);
            createdToDo.Project.Id.ShouldBe(newToDo.ProjectId);
            createdToDo.IsCompleted.ShouldBe(newToDo.IsCompleted);
        }

        [Fact]
        public async Task UpdateToDo_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultToDoToDatabase();
            var updatedToDo = new ToDoDto("New Test ToDo 2", "New Test Description 2", new DateTime(2025, 01, 01, 15, 00, 00), 2, _existingProject.Id, true);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/ToDo/{_testToDoId}", updatedToDo);

            // Assert
            response.EnsureSuccessStatusCode();
            var toDo = await response.Content.ReadFromJsonAsync<ToDoDetailsDto>();

            toDo.ShouldNotBeNull();
            toDo.Title.ShouldBe(updatedToDo.Title);
            toDo.Description.ShouldBe(updatedToDo.Description);
            toDo.DueDate.ShouldBe(updatedToDo.DueDate);
            toDo.Priority.ShouldBe(updatedToDo.Priority);
            toDo.ProjectName.ShouldBe(_existingProject.Name);
            toDo.IsCompleted.ShouldBe(updatedToDo.IsCompleted);
        }

        [Fact]
        public async Task UpdateToDo_UpdatesToDoInDatabase()
        {
            // Arrange
            await AddDefaultToDoToDatabase();
            var updatedToDo = new ToDoDto("New Test ToDo 2", "New Test Description 2", new DateTime(2025, 01, 01, 15, 00, 00), 2, _existingProject.Id, true);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/ToDo/{_testToDoId}", updatedToDo);

            // Assert
            response.EnsureSuccessStatusCode();

            var dbToDo = await _dbContext.ToDos.AsNoTracking()
                .Include(x => x.Project)
                .FirstOrDefaultAsync(toDo => toDo.Id == _testToDoId);
            dbToDo.ShouldNotBeNull();
            dbToDo.Title.ShouldBe(updatedToDo.Title);
            dbToDo.Description.ShouldBe(updatedToDo.Description);
            dbToDo.DueDate.ShouldBe(updatedToDo.DueDate);
            dbToDo.Priority.ShouldBe(updatedToDo.Priority);
            dbToDo.Project.Id.ShouldBe(updatedToDo.ProjectId);
            dbToDo.IsCompleted.ShouldBe(updatedToDo.IsCompleted);
        }

        [Fact]
        public async Task DeleteToDo_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultToDoToDatabase();

            // Act
            var response = await _client.DeleteAsync($"/api/ToDo/{_testToDoId}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteToDo_RemovesToDoInDatabase()
        {
            // Arrange
            await AddDefaultToDoToDatabase();

            // Act
            await _client.DeleteAsync($"/api/ToDo/{_testToDoId}");

            // Assert
            var heroInDb = await _dbContext.ToDos.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == _testToDoId);
            heroInDb?.ShouldBeNull();
        }

        [Fact]
        public async Task GetToDoById_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultToDoToDatabase();

            // Act
            var response = await _client.GetAsync($"/api/ToDo/{_testToDoId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var hero = await response.Content.ReadFromJsonAsync<ToDoDetailsDto>();

            hero.ShouldNotBeNull();
            hero.Id.ShouldBeEquivalentTo(_testToDoId);
        }

        [Fact]
        public async Task GetToDoById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var response = await _client.GetAsync($"/api/ToDo/{invalidId}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            // Verify the hero doesn't exist in database
            var dbToDo = await _dbContext.ToDos.AsNoTracking()
                .FirstOrDefaultAsync(hero => hero.Id == invalidId);
            dbToDo?.ShouldBeNull();
        }

        //* ProjectApi *//
        [Fact]
        public async Task GetAllProjects_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultProjectToDatabase();

            // Act
            var response = await _client.GetAsync("/api/Project");

            // Assert
            response.EnsureSuccessStatusCode();
            var project = await response.Content.ReadFromJsonAsync<List<ProjectDetailsDto>>();
            project.ShouldNotBeNull();
            project.ShouldNotBeEmpty();
            project.ShouldBeOfType<List<ProjectDetailsDto>>();
            project.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task CreateProject_ReturnsSuccess()
        {
            // Arrange
            var newProject = new ProjectDto("New Test Project", "New Test Description");

            // Act
            var response = await _client.PostAsJsonAsync("/api/Project", newProject);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void CreateProject_ReturnsErrorWhenInputIsInvalid()
        {
            // Arrange & Act
            var act = () => _client.PostAsJsonAsync("/api/Project", new ProjectDto(null));

            // Assert
            act.ShouldThrow<InvalidParameterException>($@"The argument ""name"" was not provided.");
        }

        [Fact]
        public async Task CreateProject_StoresProjectInDatabase()
        {
            // Arrange
            var newProject = new ProjectDto("New Project Name", "New Project Description");

            // Act
            var response = await _client.PostAsJsonAsync("/api/Project", newProject);
            response.EnsureSuccessStatusCode();

            // Assert
            var createdProject = await _dbContext.Projects.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Name == "New Project Name");

            createdProject.ShouldNotBeNull();
            createdProject.Name.ShouldBe(newProject.Name);
            createdProject.Description.ShouldBe(newProject.Description);
        }

        [Fact]
        public async Task UpdateProject_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultProjectToDatabase();
            var updatedProject = new ProjectDto("Project Name 123", "New Test Description 123");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/Project/{_existingProject!.Id}", updatedProject);

            // Assert
            response.EnsureSuccessStatusCode();
            var project = await response.Content.ReadFromJsonAsync<ProjectDetailsDto>();

            project.ShouldNotBeNull();
            project.Name.ShouldBe(updatedProject.Name);
            project.Description.ShouldBe(updatedProject.Description);
        }

        [Fact]
        public async Task UpdateProject_UpdatesProjectInDatabase()
        {
            // Arrange
            await AddDefaultProjectToDatabase();
            var updatedProject = new ProjectDto("Batman 1234", "New Project Description 1234");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/Project/{_existingProject!.Id}", updatedProject);

            // Assert
            response.EnsureSuccessStatusCode();

            var dbProject = await _dbContext.Projects.AsNoTracking()
                .FirstOrDefaultAsync(project => project.Id == _existingProject.Id);
            dbProject.ShouldNotBeNull();
            dbProject.Name.ShouldBe(updatedProject.Name);
            dbProject.Description.ShouldBe(updatedProject.Description);
        }

        [Fact]
        public async Task DeleteProject_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultProjectToDatabase();

            // Act
            var response = await _client.DeleteAsync($"/api/Project/{_existingProject!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteProject_RemovesProjectInDatabase()
        {
            // Arrange
            await AddDefaultProjectToDatabase();

            // Act
            await _client.DeleteAsync($"/api/Project/{_existingProject!.Id}");

            // Assert
            var projectInDb = await _dbContext.Projects.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == _existingProject.Id);
            projectInDb?.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteProject_ThrowsExceptionWhenAssociatedToDoExists()
        {
            // Arrange
            await AddDefaultToDoToDatabase();

            // Act
            var response = await _client.DeleteAsync($"/api/Project/{_existingProject!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldContain("Cannot delete a Project with ToDos associated.");

            var projectInDb = await _dbContext.Projects.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == _existingProject.Id);
            projectInDb?.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetProjectById_ReturnsSuccess()
        {
            // Arrange
            await AddDefaultProjectToDatabase();

            // Act
            var response = await _client.GetAsync($"/api/Project/{_existingProject!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var project = await response.Content.ReadFromJsonAsync<ProjectDetailsDto>();

            project.ShouldNotBeNull();
            project.Id.ShouldBeEquivalentTo(_existingProject.Id);
        }

        [Fact]
        public async Task GetProjectById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var response = await _client.GetAsync($"/api/Project/{invalidId}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            // Verify the project doesn't exist in database
            var dbProject = await _dbContext.Projects.AsNoTracking()
                .FirstOrDefaultAsync(project => project.Id == invalidId);
            dbProject?.ShouldBeNull();
        }
    }
}
