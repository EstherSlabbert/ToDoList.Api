using ToDoList.Common;
using ToDoList.DTOs;
using ToDoList.Repositories;
using ToDoList.Entities;
using static ToDoList.Specifications.OfToDos.AllSpecifications;

namespace ToDoList.Services
{
    public class ProjectService(IProjectRepository projectRepository, IToDoRepository toDoRepository) : IProjectService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IToDoRepository _toDoRepository = toDoRepository;

        public async Task<List<ProjectDetailsDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllProjectsAsync();
            var projectsDtos = projects.Select(GetProjectDetailsDto).ToList();
            return projectsDtos;
        }

        public async Task<ProjectDetailsDto?> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id) ?? throw new EntityNotFoundException($@"Project with id ""{id}"" was not found.");
            var projectDto = GetProjectDetailsDto(project);
            return projectDto;
        }

        public async Task CreateProjectAsync(ProjectDto newProjectDetails)
        {
            var newProject = new Project(newProjectDetails.Name, newProjectDetails.Description);
            await _projectRepository.AddProjectAsync(newProject);
        }

        public async Task<ProjectDetailsDto?> UpdateProjectAsync(int id, ProjectDto updateProjectDetails)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id) ?? throw new EntityNotFoundException($@"Project with id ""{id}"" was not found.");

            project.UpdateProject(updateProjectDetails.Name, updateProjectDetails.Description);

            await _projectRepository.UpdateProjectAsync(project);

            var projectDto = GetProjectDetailsDto(project);
            return projectDto;
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);

            if (project is not null)
            {
                var toDoAssociatedWithProject = await _toDoRepository.Exists(ByProject(project));
                if (toDoAssociatedWithProject)
                    throw new ProjectHasToDoAssociatedException($@"Cannot delete a Project with ToDos associated.");

                await _projectRepository.RemoveProjectAsync(id);
            }
        }

        public ProjectDetailsDto GetProjectDetailsDto(Project project)
        {
            return new ProjectDetailsDto
            (
                project.Id,
                project.Name,
                project.Description
            );
        }
    }
}
