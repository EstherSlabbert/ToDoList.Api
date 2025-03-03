using ToDoList.DTOs;

namespace ToDoList.Services
{
    public interface IProjectService
    {
        Task<List<ProjectDetailsDto>> GetAllProjectsAsync();
        Task<ProjectDetailsDto?> GetProjectByIdAsync(int id);
        Task CreateProjectAsync(ProjectDto newProjectDetails);
        Task<ProjectDetailsDto?> UpdateProjectAsync(int id, ProjectDto updateProjectDetails);
        Task DeleteProjectAsync(int id);
    }
}
