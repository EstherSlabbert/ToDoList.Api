using ToDoList.Common;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Entities;
using static ToDoList.Specifications.OfToDos.AllSpecifications;
using System.Reflection.Metadata.Ecma335;

namespace ToDoList.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IProjectRepository _projectRepository;

        public ToDoService(IToDoRepository repository, IProjectRepository projectRepository)
        {
            _toDoRepository = repository;
            _projectRepository = projectRepository;
        }

        public async Task<List<ToDoDetailsDto>> GetAllToDosAsync()
        {
            var toDos = await _toDoRepository.GetAllToDosAsync();
            var toDosDtos = toDos.Select(GetToDoDetailsDto).ToList();
            return toDosDtos;
        }

        public async Task<List<ToDoDetailsDto>> GetAllToDosInAProjectAsync(int projectId)
        {
            _ = await _projectRepository.GetProjectByIdAsync(projectId) ?? throw new EntityNotFoundException(@$"Project with Id ""{projectId}"" was not found.");
            var toDos = await _toDoRepository.Filter(ByProjectId(projectId)) ?? new List<ToDo>();
            var toDosDtos = toDos.Select(GetToDoDetailsDto).ToList();
            return toDosDtos;
        }

        public async Task<ToDoDetailsDto?> GetToDoByIdAsync(int id)
        {
            var toDo = await _toDoRepository.GetToDoByIdAsync(id) ?? throw new EntityNotFoundException($@"ToDo with Id ""{id}"" was not found.");
            var toDoDto = GetToDoDetailsDto(toDo);
            return toDoDto;
        }

        public async Task CreateToDoAsync(ToDoDto newToDoDetails)
        {
            var project = await _projectRepository.GetProjectByIdAsync(newToDoDetails.ProjectId) ?? throw new EntityNotFoundException(@$"Project with Id ""{newToDoDetails.ProjectId}"" was not found.");
            var newToDo = new ToDo(newToDoDetails.Title, newToDoDetails.Description, newToDoDetails.DueDate, newToDoDetails.Priority, project);
            await _toDoRepository.AddToDoAsync(newToDo);
        }

        public async Task<ToDoDetailsDto?> UpdateToDoAsync(int id, ToDoDto updatedToDoDetails)
        {
            var toDo = await _toDoRepository.GetToDoByIdAsync(id) ?? throw new EntityNotFoundException($@"ToDo with Id ""{id}"" was not found.");
            var project = await _projectRepository.GetProjectByIdAsync(updatedToDoDetails.ProjectId) ?? throw new EntityNotFoundException(@$"Project with Id ""{updatedToDoDetails.ProjectId}"" was not found.");

            toDo.UpdateToDo(updatedToDoDetails.Title, updatedToDoDetails.Description, updatedToDoDetails.DueDate, updatedToDoDetails.Priority, project, updatedToDoDetails.IsCompleted);

            await _toDoRepository.UpdateToDoAsync(toDo);

            var toDoDto = GetToDoDetailsDto(toDo);
            return toDoDto;
        }

        public async Task DeleteToDoAsync(int id)
        {
            _ = await _toDoRepository.GetToDoByIdAsync(id) ?? throw new EntityNotFoundException($@"ToDo with id ""{id}"" was not found.");
            await _toDoRepository.RemoveToDoAsync(id);
        }

        public ToDoDetailsDto GetToDoDetailsDto(ToDo toDo)
        {
            if (toDo.Project is null)
            {
                throw new InvalidOperationException("Project cannot be null.");
            }

            return new ToDoDetailsDto
            (
                toDo.Id,
                toDo.Title,
                toDo.Description,
                toDo.DueDate,
                toDo.Priority,
                toDo.Project.Name,
                toDo.IsCompleted
            );
        }
    }
}
