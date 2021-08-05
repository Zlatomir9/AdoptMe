﻿namespace AdoptMe.Services.Shelters
{
    public interface IShelterService
    {
        int Create(string userId, string name, string phoneNumber, string email, string cityName, string streetName, string streetNumber);

        public bool IsShelter(string userId);

        public bool RegistrationIsSubmitted(string userId);

        public bool RegistrationIsDeclined(string userId);

        public int IdByUser(string userId);

        public string EmailByUser(string userId);
    }
}
