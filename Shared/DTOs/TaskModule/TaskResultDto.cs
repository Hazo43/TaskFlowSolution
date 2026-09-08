using Shared.DTOs.Enums;

namespace Shared.DTOs.TaskModule
{
    public record TaskResultDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatusDTO StatusDTO { get; set; }
        public TaskPriorityDTO PriorityDTO { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ProjectName { get; set; }
        public string? AssignedToName { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
