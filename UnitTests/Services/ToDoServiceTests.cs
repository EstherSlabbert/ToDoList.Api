using Moq;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Services;
using ToDoList.Common;
using ToDoList.Entities;
using static ToDoList.Specifications.OfToDos.AllSpecifications;
using ToDoList.Common.Specifications;

namespace UnitTests.Services;

public class ToDoServiceTests
{
    private readonly Mock<IToDoRepository> _mockToDoRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly ToDoService _service;

    public ToDoServiceTests()
    {
        _mockToDoRepository = new Mock<IToDoRepository>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _service = new ToDoService(_mockToDoRepository.Object, _mockProjectRepository.Object);
    }

    [Fact]
    public async Task GetAllToDosAsync_ShouldReturnListOfToDosWithTheCorrectDetails()
    {
        // Arrange
        var existingProjects = new List<Project>
            {
                new()
                {
                    Id = 1,
                    Name = "Test name",
                    Description = "Test description",
                },
                new()
                {
                    Id = 2,
                    Name = "Project 2",
                    Description = ""
                }
            };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProjects.First().Id))
            .ReturnsAsync(existingProjects.First());

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProjects.Last().Id))
            .ReturnsAsync(existingProjects.Last());

        var toDos = new List<ToDo>
            {
                new() {
                    Id = 1,
                    Title = "Superman",
                    Description = "",
                    DueDate = new DateTime(2025, 02, 25, 10, 00, 00),
                    Priority = 1,
                    Project = existingProjects.First(),
                    IsCompleted = true
                },
                new() {
                    Id = 2,
                    Title = "Superman 2.0",
                    Description = "Development plan",
                    DueDate = new DateTime(2025, 02, 26, 10, 00, 00),
                    Priority = 2,
                    Project = existingProjects.Last()
                }
            };

        _mockToDoRepository.Setup(r => r.GetAllToDosAsync())
            .ReturnsAsync(toDos);

        // Act
        var result = await _service.GetAllToDosAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);

        result[0].Id.ShouldBe(toDos[0].Id);
        result[0].Title.ShouldBe(toDos[0].Title);
        result[0].Description.ShouldBe(toDos[0].Description);
        result[0].DueDate.ShouldBe(toDos[0].DueDate);
        result[0].Priority.ShouldBe(toDos[0].Priority);
        result[0].ProjectName.ShouldBe(toDos[0].Project.Name);
        result[0].IsCompleted.ShouldBe(toDos[0].IsCompleted);

        result[1].Id.ShouldBe(toDos[1].Id);
        result[1].Title.ShouldBe(toDos[1].Title);
        result[1].Description.ShouldBe(toDos[1].Description);
        result[1].DueDate.ShouldBe(toDos[1].DueDate);
        result[1].Priority.ShouldBe(toDos[1].Priority);
        result[1].ProjectName.ShouldBe(toDos[1].Project.Name);
        result[1].IsCompleted.ShouldBe(toDos[1].IsCompleted);
    }

    [Fact]
    public async Task GetAllToDosInAProjectAsync_ShouldReturnListOfToDosFromSpecifiedProjectWithTheCorrectDetails()
    {
        // Arrange
        var existingProjects = new List<Project>
            {
                new()
                {
                    Id = 1,
                    Name = "Test name",
                    Description = "Test description",
                },
                new()
                {
                    Id = 2,
                    Name = "Project 2",
                    Description = ""
                }
            };

        foreach (var project in existingProjects)
        {
            _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(project.Id))
                .ReturnsAsync(project);
        }

        var toDos = new List<ToDo>
            {
                new() {
                    Id = 1,
                    Title = "Superman",
                    Description = "",
                    DueDate = new DateTime(2025, 02, 25, 10, 00, 00),
                    Priority = 1,
                    Project = existingProjects.First(),
                    IsCompleted = true
                },
                new() {
                    Id = 2,
                    Title = "Superman 2.0",
                    Description = "Development plan",
                    DueDate = new DateTime(2025, 02, 25, 10, 00, 00),
                    Priority = 1,
                    Project = existingProjects.First()
                },
                new()
                {
                    Id = 3,
                    Title = "Outside task",
                    Description = "",
                    DueDate = new DateTime(2025, 02, 25, 10, 00, 00),
                    Priority = 5,
                    Project = existingProjects.Last()
                }
            };

        _mockToDoRepository.Setup(r => r.GetAllToDosAsync())
            .ReturnsAsync(toDos);
        _mockToDoRepository.Setup(r => r.Filter(It.IsAny<Specification<ToDo>>()))
            .ReturnsAsync(new List<ToDo> { toDos[0], toDos[1] });
        _mockToDoRepository.Setup(r => r.GetAllToDosAsync())
            .ReturnsAsync(toDos);

        // Act
        var result = await _service.GetAllToDosInAProjectAsync(existingProjects[0].Id);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);

        result[0].Id.ShouldBe(toDos[0].Id);
        result[0].Title.ShouldBe(toDos[0].Title);
        result[0].Description.ShouldBe(toDos[0].Description);
        result[0].DueDate.ShouldBe(toDos[0].DueDate);
        result[0].Priority.ShouldBe(toDos[0].Priority);
        result[0].ProjectName.ShouldBe(toDos[0].Project.Name);
        result[0].IsCompleted.ShouldBe(toDos[0].IsCompleted);

        result[1].Id.ShouldBe(toDos[1].Id);
        result[1].Title.ShouldBe(toDos[1].Title);
        result[1].Description.ShouldBe(toDos[1].Description);
        result[1].DueDate.ShouldBe(toDos[1].DueDate);
        result[1].Priority.ShouldBe(toDos[1].Priority);
        result[1].ProjectName.ShouldBe(toDos[1].Project.Name);
        result[1].IsCompleted.ShouldBe(toDos[1].IsCompleted);
    }

    [Fact]
    public async Task GetToDoByIdAsync_WithValidId_ShouldReturnToDoWithCorrectDetails()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 7,
            Name = "Test name 2.0",
            Description = "Test description 2.0",
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        var toDo = new ToDo
        {
            Id = 20,
            Title = "Supergirl",
            Description = "Work out plan",
            DueDate = new DateTime(2025, 02, 25, 10, 00, 00),
            Priority = 1,
            Project = existingProject
        };

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(toDo.Id))
            .ReturnsAsync(toDo);

        // Act
        var result = await _service.GetToDoByIdAsync(toDo.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(toDo.Id);
        result.Title.ShouldBe(toDo.Title);
        result.Description.ShouldBe(toDo.Description);
        result.DueDate.ShouldBe(toDo.DueDate);
        result.Priority.ShouldBe(toDo.Priority);
        result.IsCompleted.ShouldBe(toDo.IsCompleted);
        result.ProjectName.ShouldBe(toDo.Project.Name);
    }

    [Fact]
    public async Task GetToDoByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ToDo?)null);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.GetToDoByIdAsync(999));
    }

    [Fact]
    public async Task CreateToDoAsync_ShouldCallRepository()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 30,
            Name = "test name",
            Description = "test description"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);
        var newToDoDetails = new ToDoDto("Test 123", "Test description 123", new DateTime(2025, 02, 25, 12, 00, 00), 1, existingProject.Id);

        var newToDo = new ToDo(newToDoDetails.Title, newToDoDetails.Description, newToDoDetails.DueDate, newToDoDetails.Priority, existingProject);

        _mockToDoRepository.Setup(r => r.AddToDoAsync(newToDo))
            .ReturnsAsync(newToDo);

        // Act
        await _service.CreateToDoAsync(newToDoDetails);

        // Assert
        _mockToDoRepository.Verify(r => r.AddToDoAsync(newToDo), Times.Once);
    }

    [Fact]
    public async Task CreateToDoAsync_ShouldThrowEntityNotFoundExceptionIfProjectDoesNotExist()
    {
        // Arrange
        var nonExistingProject = new Project { Id = 12, Name = "Non-existing", Description = "" };
        var newToDoDetails = new ToDoDto("Test 23", "Test 23 description", new DateTime(2025, 02, 25, 12, 00, 00), 1, nonExistingProject.Id);
        var newToDo = new ToDo(newToDoDetails.Title, newToDoDetails.Description, newToDoDetails.DueDate, newToDoDetails.Priority, nonExistingProject, newToDoDetails.IsCompleted);

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(newToDo.Project.Id))
            .ReturnsAsync((Project?)null);
        _mockToDoRepository.Setup(r => r.AddToDoAsync(newToDo))
            .ReturnsAsync(newToDo);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.CreateToDoAsync(newToDoDetails));
    }

    [Fact]
    public async Task UpdateToDoAsync_WithValidId_ShouldReturnUpdatedToDoWithCorrectDetails()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 3,
            Name = "test name",
            Description = "test description"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        var existingToDo = new ToDo
        {
            Id = 7,
            Title = "Test",
            Description = "Test",
            DueDate = DateTime.Now,
            Priority = 1,
            Project = existingProject,
            IsCompleted = true
        };

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

        var updatedToDoDto = new ToDoDto(existingToDo.Title, existingToDo.Description, existingToDo.DueDate, existingToDo.Priority, existingToDo.Project.Id, existingToDo.IsCompleted);

        existingToDo.UpdateToDo(updatedToDoDto.Title, updatedToDoDto.Description, updatedToDoDto.DueDate, updatedToDoDto.Priority, existingProject, updatedToDoDto.IsCompleted);

        _mockToDoRepository.Setup(r => r.UpdateToDoAsync(existingToDo))
            .ReturnsAsync(existingToDo);

        // Act
        var result = await _service.UpdateToDoAsync(existingToDo.Id, updatedToDoDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingToDo.Id);
    }

    [Fact]
    public async Task UpdateToDoAsync_ShouldUpdateAllFields()
    {
        // Arrange
        var existingProject1 = new Project
        {
            Id = 3,
            Name = "test name",
            Description = "test description"
        };

        var existingProject2 = new Project
        {
            Id = 4,
            Name = "test name 2",
            Description = "test description 2"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject1.Id))
            .ReturnsAsync(existingProject1);

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject2.Id))
            .ReturnsAsync(existingProject2);

        var existingToDo = new ToDo
        {
            Id = 7,
            Title = "Test title",
            Description = "Test description",
            DueDate = new DateTime(2025, 02, 14, 12, 00, 00),
            Priority = 1,
            Project = existingProject1,
            IsCompleted = true
        };

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

        var updatedToDoDto = new ToDoDto("new title", "new description", new DateTime(2025, 02, 25, 14, 00, 00), 3, existingProject2.Id);

        existingToDo.UpdateToDo(updatedToDoDto.Title, updatedToDoDto.Description, updatedToDoDto.DueDate, updatedToDoDto.Priority, existingProject2, updatedToDoDto.IsCompleted);

        _mockToDoRepository.Setup(r => r.UpdateToDoAsync(existingToDo))
            .ReturnsAsync(existingToDo);

        // Act
        var result = await _service.UpdateToDoAsync(existingToDo.Id, updatedToDoDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingToDo.Id);
        result.Title.ShouldBe(updatedToDoDto.Title);
        result.Description.ShouldBe(updatedToDoDto.Description);
        result.DueDate.ShouldBe(updatedToDoDto.DueDate);
        result.Priority.ShouldBe(updatedToDoDto.Priority);
        result.ProjectName.ShouldBe(existingProject2.Name);
        result.IsCompleted.ShouldBe(updatedToDoDto.IsCompleted);
    }

    [Fact]
    public async Task UpdateToDoAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 3,
            Name = "test name",
            Description = "test description"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        var updatedToDoDto = new ToDoDto("new title", "new description", new DateTime(2025, 02, 25, 14, 00, 00), 3, existingProject.Id);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.UpdateToDoAsync(999, updatedToDoDto));
    }

    [Fact]
    public async Task UpdateToDoAsync_WithInvalidProjectId_ShouldThrowException()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 4,
            Name = "test name",
            Description = "test description"
        };

        var existingToDo = new ToDo
        {
            Id = 2,
            Title = "Test title",
            Description = "Test description",
            DueDate = new DateTime(2025, 02, 14, 12, 00, 00),
            Priority = 1,
            Project = existingProject,
            IsCompleted = true
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

        var nonExistingProjectId = 100;

        var updatedToDo = new ToDoDto("new title", "new description", new DateTime(2025, 02, 25, 14, 00, 00), 3, nonExistingProjectId);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.UpdateToDoAsync(existingToDo.Id, updatedToDo));
    }

    [Fact]
    public async Task DeleteToDoAsync_WithValidId_ShouldCallRepository()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 3,
            Name = "test name",
            Description = "test description"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        var existingToDo = new ToDo
        {
            Id = 10,
            Title = "Test title 10",
            Description = "Test description 10",
            DueDate = new DateTime(2025, 02, 14, 14, 00, 00),
            Priority = 1,
            Project = existingProject,
            IsCompleted = true
        };

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

        _mockToDoRepository.Setup(r => r.RemoveToDoAsync(existingToDo.Id))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteToDoAsync(existingToDo.Id);

        // Assert
        _mockToDoRepository.Verify(r => r.RemoveToDoAsync(existingToDo.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteToDoAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ToDo?)null);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.DeleteToDoAsync(999));
    }
}
