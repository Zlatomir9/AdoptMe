﻿namespace AdoptMe.Services.Shelters
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Services.Users;

    using static Common.GlobalConstants.Roles;

    public class ShelterService : IShelterService
    {
        private readonly AdoptMeDbContext data;
        private readonly IUserService userService;

        public ShelterService(AdoptMeDbContext data, IUserService userService)
        {
            this.data = data;
            this.userService = userService;
        }

        public int Create(string name, string phoneNumber, 
            string cityName, string streetName, string streetNumber)
        {
            var cityData = this.data.Cities
                .FirstOrDefault(c => c.Name == cityName);

            var addressData = this.data.Addresses
                .FirstOrDefault(a => a.StreetName == streetName && 
                                     a.StreetNumber == streetNumber);

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
                UserId = this.userService.GetUserId(),
                Name = name,
                Email = this.userService.GetUserEmail(),
                PhoneNumber = phoneNumber,
                AddressId = addressData.Id,
                RegistrationStatus = RequestStatus.Submitted
            };

            this.userService.AddUserToRole(shelterData.UserId, ShelterRoleName);

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
