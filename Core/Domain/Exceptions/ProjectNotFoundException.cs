namespace Domain.Exceptions
{
    public class ProjectNotFoundException : NotFoundExceptions
    {
        public ProjectNotFoundException(int id) : base($" Project With Id:{id} Not Found")
        {
            
        }
    }
}
