using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoList.Controllers;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Services;
using ToDoList.Entities;

namespace UnitTests.Controllers
{
    public class ProjectControllerTest
    {
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly Mock<IToDoRepository> _mockToDoRepository;
        private readonly ProjectService _service;
        private readonly ProjectController _controller;
        public ProjectControllerTest()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _mockToDoRepository = new Mock<IToDoRepository>();
            _service = new ProjectService(_mockRepository.Object, _mockToDoRepository.Object);
            _controller = new ProjectController(_service);
        }

        [Fact]
        public async Task Sends_ProjectDto_Correctly_When_Calling_CreateNewProject_Then_Returns_Ok()
        {
            //Arrange
            var requestDto = new ProjectDto("Supergirl", "Kara");
            //Act
            var response = await _controller.CreateNewProject(requestDto);
            //Assert
            response.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task Sends_Id_When_Calling_DeleteProject_Then_Returns_Ok()
        {
            //Arrange
            var existingHero = new Project
            {
                Id = 1,
                Name = "SuperGirl",
                Description = "Cara"
            };

            _mockRepository.Setup(r => r.GetProjectByIdAsync(existingHero.Id))
            .ReturnsAsync(existingHero);

            _mockRepository.Setup(r => r.RemoveProjectAsync(existingHero.Id))
            .Returns(Task.CompletedTask);

            //Act
            var response = await _controller.DeleteProject(existingHero.Id);
            //Assert
            response.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task Sends_ProjectDto_Correctly_When_Calling_UpdateProject_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingProject = new Project
            {
                Id = 1,
                Name = "SuperGirl",
                Description = "Cara"
            };
            _mockRepository.Setup(p => p.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

            var requestDto = new ProjectDto("Supergirl", "Kara");
            existingProject.UpdateProject(requestDto.Name, requestDto.Description);
            _mockRepository.Setup(r => r.UpdateProjectAsync(existingProject))
            .ReturnsAsync(existingProject);

            //Act
            var response = await _controller.UpdateProject(existingProject.Id, requestDto);
            //Assert
            response.ShouldBeOfType<ActionResult<ProjectDetailsDto>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Sends_Id_Correctly_When_Calling_GetProjectWithId_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingHero = new Project
            {
                Id = 1,
                Name = "SuperGirl",
                Description = "Cara"
            };

            _mockRepository.Setup(r => r.GetProjectByIdAsync(existingHero.Id))
            .ReturnsAsync(existingHero);

            //Act
            var response = await _controller.GetProjectWithId(existingHero.Id);
            //Assert
            response.ShouldBeOfType<ActionResult<ProjectDetailsDto>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Sends_Request_Correctly_When_Calling_GetAllProjects_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingProjects = new List<Project>
            {
                new() {
                    Id = 1,
                    Name = "Superman",
                    Description = "Clark"
                },
                new() {
                    Id = 2,
                    Name = "Batman",
                    Description = "Bruce"
                }
            };

            _mockRepository.Setup(r => r.GetAllProjectsAsync())
            .ReturnsAsync(existingProjects);

            //Act
            var response = await _controller.GetAllProjects();
            //Assert
            response.ShouldBeOfType<ActionResult<List<ProjectDetailsDto>>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }
    }
}
