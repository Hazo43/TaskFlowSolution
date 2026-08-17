namespace Domain.Entites
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!; 
    }
}
