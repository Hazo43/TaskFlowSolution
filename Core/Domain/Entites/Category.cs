namespace Domain.Entites
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } 

        // Relations

        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

    }
}
