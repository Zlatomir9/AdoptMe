namespace AdoptMe.Services.Adoptions
{
    using System;
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Users;

    using static Data.Models.Enums.RequestStatus;

    public class AdoptionService : IAdoptionService
    {
        private readonly IUserService userService;
        private readonly AdoptMeDbContext data;

        public AdoptionService(IUserService userService, AdoptMeDbContext data)
        {
            this.userService = userService;
            this.data = data;
        }

        public int CreateAdoption(string firstName, string lastName, int Age, string firstQuestion, 
            string secondQuestion, string thirdQuestion, string fourthQuestion, int petId)
        {
            var userId = this.userService.GetUserId();

            var adopter = this.data
                    .Adopters
                    .Where(x => x.UserId == userId)
                    .FirstOrDefault();

            if (adopter == null)
            {
                adopter = new Adopter
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = Age,
                    UserId = userId
                };

                this.data.Adopters.Add(adopter);
                this.data.SaveChanges();
            }

            var pet = this.data
                    .Pets
                    .Where(x => x.Id == petId && 
                           x.IsAdopted == false)
                    .FirstOrDefault();

            var adoptionData = new AdoptionApplication
            {
                AdopterId = adopter.Id,
                PetId = pet.Id,
                RequestStatus = Submitted,
                FirstAnswer = firstQuestion,
                SecondAnswer = secondQuestion,
                ThirdAnswer = thirdQuestion,
                FourthAnswer = firstQuestion,
                SubmittedOn = DateTime.UtcNow
            };

            this.data.AdoptionApplications.Add(adoptionData);
            this.data.SaveChanges();

            return adoptionData.PetId;
        }

        public bool SentApplication(int id)
            => this.data
                .AdoptionApplications
                .Any(x => x.Adopter.UserId == userService.GetUserId() &&
                    x.PetId == id);
    }
}
