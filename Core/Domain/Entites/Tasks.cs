using Domain.Entites.Enums;
using TaskStatus = Domain.Entites.Enums.TaskStatus;

namespace Domain.Entites
{
    public class Tasks : BaseEntity<int>
    {

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



        #region Relations
        // 1 -> Task , m -> comment
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // m -> Task , 1 -> Category
        public Category? Category { get; set; }
        public int? CategoryId { get; set; }


        // m -> Task , 1 -> User
        public User? AssignedTo { get; set; }    // user 
        public int? AssignedToId { get; set; } // References User

        // 
        // m -> Task , 1 -> Project
        public Project Project { get; set; } = default!;
        public int ProjectId { get; set; } // References Project — required

        #endregion

    }
}
