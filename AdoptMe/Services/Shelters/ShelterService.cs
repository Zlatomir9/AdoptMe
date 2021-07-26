namespace AdoptMe.Services.Shelters
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using System.Linq;

    public class ShelterService : IShelterService
    {
        private readonly AdoptMeDbContext data;

        public ShelterService(AdoptMeDbContext data)
            => this.data = data;

        public int Create(string userId, string name, string phoneNumber, string email)
        {
            var shelterData = new Shelter
            {
                UserId = userId,
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email
            };

            this.data.Shelters.Add(shelterData);
            this.data.SaveChanges();

            return shelterData.Id;
        }

        public int IdByUser(string userId)
            => this.data
                   .Shelters
                   .Where(s => s.UserId == userId)
                   .Select(s => s.Id)
                   .FirstOrDefault();

        public string EmailByUser(string userId)
            => this.data
                   .Shelters
                   .Where(s => s.UserId == userId)
                   .Select(s => s.Email)
                   .FirstOrDefault();

        public bool IsShelter(string userId)
            => this.data
                   .Shelters
                   .Any(s => s.UserId == userId);
    }
}
