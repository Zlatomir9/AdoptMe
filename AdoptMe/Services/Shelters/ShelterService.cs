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

        public int Create(string userId, string name, string phoneNumber, 
            string email, string cityName, string streetName, string streetNumber)
        {
            var cityData = this.data.Cities.FirstOrDefault(c => c.Name == cityName);
            var addressData = this.data.Addresses.FirstOrDefault(a => a.StreetName == streetName && a.StreetNumber == streetNumber);

            if (cityData == null)
            {
                cityData = new City
                {
                    Name = cityName
                };

                this.data.Cities.Add(cityData);
                this.data.SaveChanges();
            }

            if (addressData == null)
            {
                addressData = new Address
                {
                    StreetName = streetName,
                    StreetNumber = streetNumber,
                    CityId = cityData.Id
                };

                cityData.Addresses.Add(addressData);

                this.data.Addresses.Add(addressData);
                this.data.SaveChanges();
            }

            var shelterData = new Shelter
            {
                UserId = userId,
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                AddressId = addressData.Id,
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
