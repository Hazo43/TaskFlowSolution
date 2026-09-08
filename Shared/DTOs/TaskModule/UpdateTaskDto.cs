using Shared.DTOs.Enums;

namespace Shared.DTOs.TaskModule
{
    public record UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriorityDTO Priority { get; set; }
        public DateTime? DueDate { get; set; }

        public int? AssignedToId { get; set; }
        public int? CategoryId { get; set; }
    }
}
