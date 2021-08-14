namespace AdoptMe.Services.Statistics
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Models.Home;

    using static Data.Models.Enums.RequestStatus;

    public class StatisticsService : IStatisticsService
    {
        private readonly AdoptMeDbContext data;

        public StatisticsService(AdoptMeDbContext data)
            => this.data = data;

        public StatisticsViewModel Total()
        {
            var totalPets = this.data.Pets.Where(p => p.IsAdopted == false && p.IsDeleted == false).Count();
            var totalShelters = this.data.Shelters.Where(s => s.RegistrationStatus == Аccepted).Count();
            var totalAdoptions = this.data.AdoptionApplications.Where(a => a.RequestStatus == Аccepted).Count();

            return new StatisticsViewModel
            {
                TotalPets = totalPets,
                TotalShelters = totalShelters,
                TotalAdoptions = totalAdoptions
            };
        }
    }
}
