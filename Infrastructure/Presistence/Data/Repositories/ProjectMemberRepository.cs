using Domain.Entites;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DbContexts;

namespace Presistence.Data.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly TaskFlowDbContext _dbContext;

        public ProjectMemberRepository(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddProjectMemberAsync(ProjectMember projectMember)
        {
           await _dbContext.ProjectMembers.AddAsync(projectMember);
        }

        public async Task<ProjectMember?> GetProjectMemberAsync(int projectId, int userId)
        {
            return await _dbContext.ProjectMembers.FirstOrDefaultAsync
                (x =>
                x.ProjectId == projectId &&
                x.UserId == userId
                );
        }

        public void RemoveProjectMember(ProjectMember projectMember)
        {
             _dbContext.Remove(projectMember);
        }
    }
}
