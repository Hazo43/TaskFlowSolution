using Shared.DTOs.Project;

namespace Services.Abstraction.Interfaces
{
    public interface IProjectService
    {
        // Get All Projects
        Task<IEnumerable<ProjectResultDto>> GetAllProjectAsync();

        // Get By Id 
        Task<ProjectResultDto> GetByIdAsync(int id);

        // Create Project
        Task<ProjectResultDto> CreateProjectAsync(CreateProjectDto createDto);

        // Update Project
        Task<ProjectResultDto> UpdateProjectAsync(int id , UpdateProjectDto updateDto);

        // Delete Project 
        Task DeleteAsync(int id);

        // Add Member
        Task AddMemberAsync(int projectId, int userId);

        // Delete Member
        Task RemoveMemberAsync(int projectId, int userId);

    }
}
