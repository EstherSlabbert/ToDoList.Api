using ToDoList.Common.Specifications;
using ToDoList.Entities;

namespace ToDoList.Repositories
{
    public interface IToDoRepository
    {
        Task<List<ToDo>> GetAllToDosAsync();
        Task<ToDo?> GetToDoByIdAsync(int id);
        Task<ToDo> AddToDoAsync(ToDo newToDo);
        Task<ToDo?> UpdateToDoAsync(ToDo updatedToDo);
        Task RemoveToDoAsync(int id);
        Task<bool> Exists(params Specification<ToDo>[] filter);
        Task<List<ToDo>> Filter(params Specification<ToDo>[] filter);
    }
}
