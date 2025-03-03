using Microsoft.AspNetCore.Mvc;
using ToDoList.DTOs;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDetailsDto>>> GetAllProjects()
        {
            var projectDtos = await _projectService.GetAllProjectsAsync();
            return Ok(projectDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDetailsDto>> GetProjectWithId(int id)
        {
            var projectDto = await _projectService.GetProjectByIdAsync(id);
            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewProject(ProjectDto projectDto)
        {
            await _projectService.CreateProjectAsync(projectDto);
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectDetailsDto>> UpdateProject([FromRoute] int id, [FromBody] ProjectDto updateProjectDto)
        {
            var updatedProjectDetailsDto = await _projectService.UpdateProjectAsync(id, updateProjectDto);
            return Ok(updatedProjectDetailsDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProject([FromRoute] int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return Ok();
        }
    }
}
