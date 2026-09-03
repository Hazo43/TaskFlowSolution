namespace Shared.DTOs.Project
{
    public record UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
