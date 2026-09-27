namespace Shared.DTOs.CommentModule
{
    public sealed class CommentResultDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AuthorName { get; set; } 
        public string? TaskTitle { get; set; } 
    }
}
