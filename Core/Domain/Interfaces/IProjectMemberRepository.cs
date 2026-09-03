using Domain.Entites;

namespace Domain.Interfaces
{
    public interface IProjectMemberRepository
    {
        // هات الـ Member اللي موجود في Project معين والـ User بتاعه معين.
        Task<ProjectMember?> GetProjectMemberAsync(int projectId, int userId);
        Task AddProjectMemberAsync(ProjectMember projectMember);
        void RemoveProjectMember(ProjectMember projectMember);

    }
}
