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

        // ProjectMember  فيها موجود ك userId اللي ال Projects دي ببساطه بتقول هات كل ال
        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
        {
            // UserId == userId اللي ال Project هتجيب كل ال
             // admin مثلا عندو اكتر من واحد هترجعهم كلهم لي ؟ لانو hazo يعني 
            return await _dbContext.ProjectMembers.Where(x => x.UserId == userId)
                                                  .Select(x => x.Project).ToListAsync();
        }

    }
}
