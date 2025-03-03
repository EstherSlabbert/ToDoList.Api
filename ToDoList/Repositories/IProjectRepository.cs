using ToDoList.Entities;

namespace ToDoList.Repositories
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> AddProjectAsync(Project newProject);
        Task<Project?> UpdateProjectAsync(Project updateProject);
        Task RemoveProjectAsync(int id);
    }
}
