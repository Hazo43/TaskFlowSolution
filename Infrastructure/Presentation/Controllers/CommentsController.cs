using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Interfaces;
using Shared.DTOs.CommentModule;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api")]

    //  /api/comments/tasks/{taskId}/comments  <-  Route هيبقى الـ BaseApiController لو ورثت من   
    // BaseApiController و دا غلط عشان كدا مورثتش من ال 
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {

            _commentService = commentService;
        }


        // GET : BaseApi/Comments/{taskId}/comments
        [HttpGet("tasks/{taskId}/comments")]
        public async Task<ActionResult<IEnumerable<CommentResultDTO>>> GetCommentByTaskId(int taskId)
        {
            var comment = await _commentService.GetCommentsByTaskId(taskId);
            return Ok(comment);
        }


       
        // POST : BaseApi/Comments/{taskId}/comments
        [HttpPost("tasks/{taskId}/comments")]
        public async Task<ActionResult<CommentResultDTO>> CreateComment(int taskId, CreateCommentDTO createCommentDTO)
        {
            var comment = await _commentService.CreateComment(taskId, createCommentDTO);
            return Ok(comment);
        }

      
        
        // Put : BaseApi/comments/{commentId}
        [HttpPut("comments/{commentId:int}")]
        public async Task<ActionResult<CommentResultDTO>> UpdateComment(int commentId , UpdateCommentDTO updateCommentDTO)
        {
            var comment = await _commentService.UpdateComment(commentId, updateCommentDTO);
            return Ok(comment);
        }

        
        // Delete : BaseApi/comments/{commentId}
        [HttpDelete("comments/{commentId:int}")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            await _commentService.DeleteComment(commentId);
            return NoContent();
        }
    }
}
