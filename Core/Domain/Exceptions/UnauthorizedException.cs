namespace Domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = $"Something is wrong") : base(message)
        {
            
        }
    }
}
