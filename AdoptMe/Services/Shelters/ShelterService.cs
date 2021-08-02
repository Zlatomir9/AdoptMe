namespace AdoptMe.Services.Shelters
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;

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
                Email = email,
                RegistrationStatus = RegistrationStatus.Submitted
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
                   .Any(s => s.UserId == userId && s.RegistrationStatus == RegistrationStatus.Аccepted);

        public bool RegistrationIsSubmitted(string userId)
            => this.data
                   .Shelters
                   .Any(s => s.UserId == userId && s.RegistrationStatus == RegistrationStatus.Submitted);

        public bool RegistrationIsDeclined(string userId)
            => this.data
                   .Shelters
                   .Any(s => s.UserId == userId && s.RegistrationStatus == RegistrationStatus.Declined);
    }
}
