namespace Domain.Entites
{
    public class ProjectMember
    {

        public DateTime JoinedAt { get; set; }

        public User User { get; set; } = default!;
      
        //UserId int (FK) References User — Composite PK
        public int UserId { get; set; }
        public Project Project { get; set; } = default!;

        // ProjectId int (FK) References Project — Composite PK
        public int ProjectId { get; set; }

    }
}
