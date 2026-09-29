using Shared.DTOs.CommentModule;

namespace Services.Abstraction.Interfaces
{
    public interface ICommentService
    {


        // Get Task By Id
        Task<IEnumerable<CommentResultDTO>> GetCommentsByTaskId(int taskId);

        // Create Comment
        Task<CommentResultDTO> CreateComment(int taskId, CreateCommentDTO createCommentDTO);

        // Update Comment
        Task<CommentResultDTO> UpdateComment(int commentId, UpdateCommentDTO updateCommentDTO);

        // Delet Comment
        Task DeleteComment(int commentId);
    }
}
