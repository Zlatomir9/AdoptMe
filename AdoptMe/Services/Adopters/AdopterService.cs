namespace AdoptMe.Services.Adopters
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Services.Users;
    
    using static Common.GlobalConstants.Roles;

    public class AdopterService : IAdopterService
    {
        private readonly AdoptMeDbContext data;
        private readonly IUserService userService;

        public AdopterService(AdoptMeDbContext data, IUserService userService)
        {
            this.data = data;
            this.userService = userService;
        }

        public int Create(string firstName, string lastName, int age, string userId)
        {
            var adopterData = new Adopter
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                UserId = userId
            };

            this.data.Adopters.Add(adopterData);
            this.userService.AddUserToRole(adopterData.UserId, AdopterRoleName);

            this.data.SaveChanges();

            return adopterData.Id;
        }

        public bool IsAdopter(string userId)
            => this.data
                   .Adopters
                   .Any(x => x.UserId == userId);
    }
}
