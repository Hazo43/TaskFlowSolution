namespace Domain.Exceptions
{
    public class CommentNotFoundException : NotFoundExceptions
    {
        public CommentNotFoundException(int id) : base($"Comment With Id:{id} Not Found")
        {
            
        }
    }
}
