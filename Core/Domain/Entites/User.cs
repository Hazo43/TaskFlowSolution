using Microsoft.AspNetCore.Identity;

namespace Domain.Entites
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        #region Relations

        // 1 -> User  m -> Commment
        public ICollection<Comment> Comments { get; set; } = default!;

        // 1 -> User  m -> ProectMember
        public ICollection<ProjectMember> ProjectMembers { get; set; } = default!;

        // 1 -> User  m -> Task
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

        // 1 -> User  m -> Task
        public ICollection<Project> Projects { get; set; } = default!;



        #endregion
    }
}
