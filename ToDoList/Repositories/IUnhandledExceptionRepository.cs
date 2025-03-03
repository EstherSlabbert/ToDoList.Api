using ToDoList.Entities;

namespace ToDoList.Repositories
{
    public interface IUnhandledExceptionRepository
    {
        Task<int> RecordAsync(UnhandledException exception);
    }
}
