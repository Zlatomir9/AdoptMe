namespace AdoptMe.Services.Adopters
{
    public interface IAdopterService
    {
        int Create(string firstName, string lastName, int Age, string userId);

        bool IsAdopter(string userId);
    }
}