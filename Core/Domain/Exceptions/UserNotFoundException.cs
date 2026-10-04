namespace Domain.Exceptions
{
    public class UserNotFoundException : NotFoundExceptions
    {
        public UserNotFoundException(int id) : base($"User With Id:{id} Not Found")
        {
            
        }
    }
}
