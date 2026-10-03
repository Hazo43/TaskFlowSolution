namespace Domain.Exceptions
{
    public abstract class NotFoundExceptions : Exception
    {
        protected NotFoundExceptions( string message) : base(message)
        {
            
        }
    }
}
