namespace Domain.Exceptions
{
    public class CategoryNotFoundException : NotFoundExceptions
        
    {
        public CategoryNotFoundException(int id) : base($"Category With:{id} Not Found")
        {
            
        }
    }
}
