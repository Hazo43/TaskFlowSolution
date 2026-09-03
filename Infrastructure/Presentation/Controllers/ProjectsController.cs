using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Interfaces;
using Shared.DTOs.Project;

namespace Presentation.Controllers
{
    public class ProjectsController : BaseApiController
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{id}")] //BaseUrl/api/Projects/{id}
        public async Task<ActionResult<ProjectResultDto>> GetProjectById(int id)
        {
            var projectId = await _projectService.GetByIdAsync(id);
            return Ok(projectId);
        }

        [HttpGet]   // BaseUrl/api/Projects
        public async Task<ActionResult<IEnumerable<ProjectResultDto>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectAsync();
            return Ok(projects);
        }

        [HttpPost]   // BaseUrl/api/Projects
        public async Task<ActionResult<ProjectResultDto>> CreateProject(CreateProjectDto createProjectDto)
        {
            return Ok(await _projectService.CreateProjectAsync(createProjectDto));
        }
        
        [HttpPut("{id}")]  // BaseUrl/api/Projects/{id}
        public async Task<ActionResult<ProjectResultDto>> UpdateProject(int id, UpdateProjectDto updateProjectDto)
        {
            return Ok(await _projectService.UpdateProjectAsync(id, updateProjectDto));
        }

        [HttpDelete("{id}")] // BaseUrl/api/Projects/{id}
        public async Task<ActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{projectId}/members/{userId}")] // BaseUrl/api/Projects/projectId/members/userId
        public async Task<ActionResult> AddProjectMember(int projectId, int userId)
        {
            await _projectService.AddMemberAsync(projectId, userId);
            return Ok();
        }

        [HttpDelete("{projectId}/members/{userId}")] // BaseUrl/api/Projects/projectId/members/userId
        public async Task<ActionResult> DeleteProjectMember(int projectId, int userId)
        {
            await _projectService.RemoveMemberAsync(projectId, userId);
            return NoContent();
        }
    }
}
