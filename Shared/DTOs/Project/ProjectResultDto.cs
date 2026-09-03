namespace Shared.DTOs.Project
{
    public record ProjectResultDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int OwnerId { get; set; }
    }
}
