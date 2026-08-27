namespace Domain.Interfaces
{
    public interface IDataSeeding
    {
        Task SeedIdentityDataAsync();
        Task DataSeedAsync();
    }
}
