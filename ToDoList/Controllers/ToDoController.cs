using Microsoft.AspNetCore.Mvc;
using ToDoList.DTOs;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToDoDetailsDto>>> GetAllToDos()
        {
            var toDoDtos = await _toDoService.GetAllToDosAsync();
            return Ok(toDoDtos);
        }

        [Route("Project/{projectId:int}")]
        [HttpGet]
        public async Task<ActionResult<List<ToDoDetailsDto>>> GetAllToDosInAProject(int projectId)
        {
            var toDoDtos = await _toDoService.GetAllToDosInAProjectAsync(projectId);
            return Ok(toDoDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ToDoDetailsDto>> GetToDoById([FromRoute] int id)
        {
            var toDoDto = await _toDoService.GetToDoByIdAsync(id);
            return Ok(toDoDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewToDo(ToDoDto toDoDto)
        {
            await _toDoService.CreateToDoAsync(toDoDto);
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ToDoDetailsDto>> UpdateToDo([FromRoute] int id, [FromBody] ToDoDto updateToDoDetails)
        {
            var updatedToDoDto = await _toDoService.UpdateToDoAsync(id, updateToDoDetails);
            return Ok(updatedToDoDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteToDo([FromRoute] int id)
        {
            await _toDoService.DeleteToDoAsync(id);
            return Ok();
        }
    }
}
