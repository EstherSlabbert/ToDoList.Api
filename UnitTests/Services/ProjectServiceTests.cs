using Moq;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Services;
using ToDoList.Common;
using ToDoList.Entities;
using static ToDoList.Specifications.OfToDos.AllSpecifications;
using ToDoList.Common.Specifications;

namespace UnitTests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<IToDoRepository> _mockToDoRepository;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockToDoRepository = new Mock<IToDoRepository>();
        _service = new ProjectService(_mockProjectRepository.Object, _mockToDoRepository.Object);
    }

    [Fact]
    public async Task GetAllProjectsAsync_ShouldReturnListOfProjectsWithTheCorrectDetails()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() {
                Id = 1,
                Name = "Project Alpha",
                Description = "Project Alpha's mission is to complete xyz."
            },
            new() {
                Id = 2,
                Name = "Project Beta",
                Description = ""
            }
        };

        _mockProjectRepository.Setup(r => r.GetAllProjectsAsync())
            .ReturnsAsync(projects);

        // Act
        var result = await _service.GetAllProjectsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);

        result[0].Id.ShouldBe(projects[0].Id);
        result[0].Name.ShouldBe(projects[0].Name);
        result[0].Description.ShouldBe(projects[0].Description);

        result[1].Id.ShouldBe(projects[1].Id);
        result[1].Name.ShouldBe(projects[1].Name);
        result[1].Description.ShouldBe(projects[1].Description);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithValidId_ShouldReturnProjectWithCorrectDetails()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Project Omega",
            Description = "Omega description"
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(project.Id))
            .ReturnsAsync(project);

        // Act
        var result = await _service.GetProjectByIdAsync(project.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(project.Id);
        result.Name.ShouldBe(project.Name);
        result.Description.ShouldBe(project.Description);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Project?)null);

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.GetProjectByIdAsync(999));
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldCallRepository()
    {
        // Arrange
        var newProjectDto = new ProjectDto("Flash Project", "Find Barry Allen.");

        var newProject = new Project(newProjectDto.Name, newProjectDto.Description);

        _mockProjectRepository.Setup(r => r.AddProjectAsync(newProject))
            .ReturnsAsync(newProject);

        // Act
        await _service.CreateProjectAsync(newProjectDto);

        // Assert
        _mockProjectRepository.Verify(r => r.AddProjectAsync(newProject), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithValidId_ShouldReturnUpdatedProjectWithCorrectDetails()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 7,
            Name = "Wonder Woman Project",
            Description = "Empower women"
        };
        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);
        var updatedProjectDto = new ProjectDto(existingProject.Name, existingProject.Description);

        existingProject.UpdateProject(updatedProjectDto.Name, updatedProjectDto.Description);
        _mockProjectRepository.Setup(r => r.UpdateProjectAsync(existingProject))
            .ReturnsAsync(existingProject);

        // Act
        var result = await _service.UpdateProjectAsync(existingProject.Id, updatedProjectDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingProject.Id);
        result.Name.ShouldBe(existingProject.Name);
        result.Description.ShouldBe(existingProject.Description);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateAllFields()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 2,
            Name = "Iron Man Project",
            Description = "Exercise plan"
        };
        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);
        var updatedProjectDto = new ProjectDto("Iron Man 2.0", "Exercises");

        existingProject.UpdateProject(updatedProjectDto.Name, updatedProjectDto.Description);
        _mockProjectRepository.Setup(r => r.UpdateProjectAsync(existingProject))
            .ReturnsAsync(existingProject);

        // Act
        var result = await _service.UpdateProjectAsync(existingProject.Id, updatedProjectDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(existingProject.Id);
        result.Name.ShouldBe(updatedProjectDto.Name);
        result.Description.ShouldBe(updatedProjectDto.Description);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updatedProject = new ProjectDto("Batman", "Batman project description");

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _service.UpdateProjectAsync(999, updatedProject));
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldThrowProjectHasToDoAssociatedException_WhenProjectHasExistingAssociatedToDo()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 12,
            Name = "Test name 44",
            Description = "Test description 44",
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        var associatedToDo = new ToDo
        {
            Id = 20,
            Title = "Associated Title",
            Description = "Associated description",
            DueDate = new DateTime(2025, 02, 12, 12, 15, 00),
            Project = existingProject,
            Priority = 1
        };

        _mockToDoRepository.Setup(r => r.GetToDoByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(associatedToDo);

        _mockToDoRepository.Setup(r => r.Exists(It.IsAny<Specification<ToDo>>()))
            .ReturnsAsync(true);
        // Act & Assert
        await Should.ThrowAsync<ProjectHasToDoAssociatedException>(async () =>
            await _service.DeleteProjectAsync(existingProject.Id));
    }

    [Fact]
    public async Task DeleteProjectAsync_WithValidId_ShouldCallRepository()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 10,
            Name = "Spiderman",
            Description = "Spiderman project description."
        };

        _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

        _mockProjectRepository.Setup(r => r.RemoveProjectAsync(existingProject.Id))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteProjectAsync(existingProject.Id);

        // Assert
        _mockProjectRepository.Verify(r => r.RemoveProjectAsync(existingProject.Id), Times.Once);
    }
}
