using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Interfaces;
using Shared.DTOs;
using Shared.DTOs.TaskModule;

namespace Presentation.Controllers
{
    public class TasksController : BaseApiController
    {
       
        private readonly ITaskService _taskService;

        public TasksController( ITaskService taskService)
        {
            _taskService = taskService;
        }

        // EndPoint => Get All Tasks
        // Get : BaseUrl/api/Tasks

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResultDto>>> GetAllTasks([FromQuery] TaskSpecificationParameter parameter )
        {
            var tasks = await _taskService.GetAllAsync(parameter);
            return Ok(tasks);
        }

        // EndPoint => Get Task by Id
        // Get : BaseUrl/api/Tasks/{id}

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskResultDto>> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return Ok(task);
        }


        // EndPoint => Post Create Task
        // Post : BaseUrl/api/Tasks

        [HttpPost]
        public async Task<ActionResult<TaskResultDto>> CreateTask(CreateTaskDto createTaskDto)
        {
            var task = await _taskService.CreateAsync(createTaskDto);
            return Ok(task);
        }


        // EndPoint => Update Task
        // PUT : BaseUrl/api/Tasks/{id}

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskResultDto>> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            var task = await _taskService.UpdateAsync(id, updateTaskDto);
            return Ok(task);
        }


        // EndPoint => Update Task Status
        // PATCH : BaseUrl/api/Tasks/{id}/status

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<TaskResultDto>> UpdateTaskStatus(UpdateTaskStatusDto updateTaskStatusDto, int id)
        {
            var task = await _taskService.UpdateTaskStatus(updateTaskStatusDto, id);
            return Ok(task);
        }


        // EndPoint => Delete Task by Id
        // Delete : BaseUrl/api/Tasks/{id}

        [HttpDelete("{id:int}")]
        public  async Task<ActionResult> DeleteById(int id)
        {
            await _taskService.Delete(id);
            return NoContent();
        }
    }
}
