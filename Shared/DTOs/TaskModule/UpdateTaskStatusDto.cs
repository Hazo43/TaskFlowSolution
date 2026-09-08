using Shared.DTOs.Enums;

namespace Shared.DTOs.TaskModule
{
    public record UpdateTaskStatusDto
    {
        public TaskStatusDTO Status { get; set; }
    }
}
