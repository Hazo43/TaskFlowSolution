namespace Domain.Entites
{
    public class Comment : BaseEntity<int>
    {

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //        TaskId int (FK) References Task
        //AuthorId int (FK) References ApplicationUser


        #region Relations

        // 1 -> User , m -> Comment
        // AuthorId (FK) -> Reference from User

        public User Author { get; set; } = default!;
        public int AuthorId { get; set; }

        // 1 -> Task , m -> Comments
        // TaskId (FK) -> Reference from Task

        public Tasks Task { get; set; } = default!;
        public int TaskId { get; set; }
        
        #endregion

    }
}
