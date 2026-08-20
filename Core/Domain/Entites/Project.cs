using Domain.Entites.Enums;

namespace Domain.Entites
{
    public class Project : BaseEntity<int>
    {


        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        #region Relations 

        // 1 -> User , M -> Project
        public User Owner { get; set; } = default!;  // User
        public int OwnerId { get; set; } // (FK) References User

        // 1 -> project , m -> ProjectMember
        public ICollection<ProjectMember> ProjectMembers { get; set; } = default!;

        //  1 -> project , m -> Task
        public ICollection<Tasks> Tasks { get; set; } =  new List<Tasks>();


        #endregion

    }
}
