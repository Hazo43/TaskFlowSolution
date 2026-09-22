using Shared.DTOs;
using Shared.DTOs.TaskModule;

namespace Services.Abstraction.Interfaces
{
    public interface ITaskService
    {
        // Get All Tasks
        Task<IEnumerable<TaskResultDto>> GetAllAsync(TaskSpecificationParameter parameter);

        // Get By Id 
        Task<TaskResultDto> GetByIdAsync(int id);

        // Create Tasks
        Task<TaskResultDto> CreateAsync(CreateTaskDto createTaskDto);

        // Update Task
        Task<TaskResultDto> UpdateAsync(int taskid, UpdateTaskDto updateTaskDto);

        // Update Task Status
        Task<TaskResultDto> UpdateTaskStatus(UpdateTaskStatusDto updateTaskStatusDto, int id);

        // Delete Task
        Task Delete(int id);
    }
}
