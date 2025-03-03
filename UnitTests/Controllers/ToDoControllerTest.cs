using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoList.Controllers;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Services;
using ToDoList.Entities;

namespace UnitTests.Controllers
{
    public class ToDoControllerTest
    {
        private readonly Mock<IToDoRepository> _mockRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly ToDoService _service;
        private readonly ToDoController _controller;
        public ToDoControllerTest()
        {
            _mockRepository = new Mock<IToDoRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _service = new ToDoService(_mockRepository.Object, _mockProjectRepository.Object);
            _controller = new ToDoController(_service);
        }

        [Fact]
        public async Task Sends_ToDoDto_Correctly_When_Calling_CreateNewToDo_Then_Returns_Ok()
        {
            //Arrange
            var existingProject = new Project { Id = 1, Name = "Default Project", Description = "" };

            _mockProjectRepository.Setup(p => p.GetProjectByIdAsync(existingProject.Id))
            .ReturnsAsync(existingProject);

            var requestDto = new ToDoDto("Title", "Description", new DateTime(), 2, 1);
            //Act
            var response = await _controller.CreateNewToDo(requestDto);
            //Assert
            response.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task Sends_Id_When_Calling_DeleteToDo_Then_Returns_Ok()
        {
            //Arrange
            var existingProject = new Project { Id = 1, Name = "Default Project", Description = "" };

            _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
                .ReturnsAsync(existingProject);

            var existingToDo = new ToDo
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                DueDate = new DateTime(2025, 01, 01, 12, 00, 00),
                Project = existingProject,
                Priority = 1,
                IsCompleted = false
            };

            _mockRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

            _mockRepository.Setup(r => r.RemoveToDoAsync(existingToDo.Id))
            .Returns(Task.CompletedTask);

            //Act
            var response = await _controller.DeleteToDo(existingToDo.Id);
            //Assert
            response.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task Sends_ToDoDto_Correctly_When_Calling_UpdateToDo_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingProject = new Project { Id = 1, Name = "Default Project", Description = "" };

            _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
                .ReturnsAsync(existingProject);

            var existingToDo = new ToDo
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                DueDate = new DateTime(2025, 01, 01, 12, 00, 00),
                Project = existingProject,
                Priority = 1,
                IsCompleted = false
            };
            _mockRepository.Setup(t => t.GetToDoByIdAsync(existingToDo.Id))
                .ReturnsAsync(existingToDo);

            var requestDto = new ToDoDto("New Test", "New Description", new DateTime(2025, 02, 01, 12, 00, 00), 2, existingProject.Id, true);

            existingToDo.UpdateToDo(requestDto.Title, requestDto.Description, requestDto.DueDate, requestDto.Priority, existingProject, requestDto.IsCompleted);
            
            _mockRepository.Setup(r => r.UpdateToDoAsync(existingToDo))
                .ReturnsAsync(existingToDo);

            //Act
            var response = await _controller.UpdateToDo(existingToDo.Id, requestDto);
            //Assert
            response.ShouldBeOfType<ActionResult<ToDoDetailsDto>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Sends_Id_Correctly_When_Calling_GetToDoWithId_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingProject = new Project { Id = 1, Name = "Default Project", Description = "" };

            _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
                .ReturnsAsync(existingProject);

            var existingToDo = new ToDo
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                DueDate = new DateTime(2025, 01, 01, 12, 00, 00),
                Project = existingProject,
                Priority = 1,
                IsCompleted = false
            };

            _mockRepository.Setup(r => r.GetToDoByIdAsync(existingToDo.Id))
            .ReturnsAsync(existingToDo);

            //Act
            var response = await _controller.GetToDoById(existingToDo.Id);
            //Assert
            response.ShouldBeOfType<ActionResult<ToDoDetailsDto>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Sends_Request_Correctly_When_Calling_GetAllToDos_Then_Returns_OkObjectResult()
        {
            //Arrange
            var existingProject = new Project { Id = 1, Name = "Default Project", Description = "" };

            _mockProjectRepository.Setup(r => r.GetProjectByIdAsync(existingProject.Id))
                .ReturnsAsync(existingProject);

            var existingToDos = new List<ToDo>
            {
                new() {
                    Id = 1,
                    Title = "Test",
                    Description = "Test",
                    DueDate = new DateTime(2025, 01, 01, 12, 00, 00),
                    Project = existingProject,
                    Priority = 1,
                    IsCompleted = false
                },
                new() {
                    Id = 2,
                    Title = "Test 2",
                    Description = "Test 2",
                    DueDate = new DateTime(2025, 02, 01, 12, 00, 00),
                    Project = existingProject,
                    Priority = 2,
                    IsCompleted = false
                }
            };

            _mockRepository.Setup(r => r.GetAllToDosAsync())
            .ReturnsAsync(existingToDos);

            //Act
            var response = await _controller.GetAllToDos();
            //Assert
            response.ShouldBeOfType<ActionResult<List<ToDoDetailsDto>>>();
            response.Result.ShouldBeOfType<OkObjectResult>();
        }
    }
}
