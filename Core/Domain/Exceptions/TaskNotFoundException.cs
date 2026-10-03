namespace Domain.Exceptions
{
    public sealed class TaskNotFoundException : NotFoundExceptions
    {
        public TaskNotFoundException(int id) : base($" Task with Id:{id} Not Found")
        {

        }
    }
}
