using ToDoList.DTOs;

namespace ToDoList.Services
{
    public interface IToDoService
    {
        Task<List<ToDoDetailsDto>> GetAllToDosAsync();
        Task<List<ToDoDetailsDto>> GetAllToDosInAProjectAsync(int projectId);
        Task<ToDoDetailsDto?> GetToDoByIdAsync(int id);
        Task CreateToDoAsync(ToDoDto newToDoDetails);
        Task<ToDoDetailsDto?> UpdateToDoAsync(int id, ToDoDto updateToDoDetails);
        Task DeleteToDoAsync(int id);
    }
}
