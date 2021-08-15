namespace AdoptMe.Services.Shelters
{
    using System.Linq;
    using System.Threading.Tasks;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using Microsoft.AspNetCore.Identity;

    using static Common.GlobalConstants.Roles;

    public class ShelterService : IShelterService
    {
        private readonly AdoptMeDbContext data;
        private readonly UserManager<User> userManager;

        public ShelterService(AdoptMeDbContext data, UserManager<User> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

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
                RegistrationStatus = RequestStatus.Submitted
            };

            Task
                .Run(async () =>
                {
                    var user = this.userManager.FindByIdAsync(shelterData.UserId).Result;
                    await userManager.AddToRoleAsync(user, ShelterRoleName);
                })
                .GetAwaiter()
                .GetResult();

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
                   .Any(s => s.UserId == userId && s.RegistrationStatus == RequestStatus.Аccepted);

        public bool RegistrationIsSubmitted(string userId)
            => this.data
                   .Shelters
                   .Any(s => s.UserId == userId && s.RegistrationStatus == RequestStatus.Submitted);
    }
}
