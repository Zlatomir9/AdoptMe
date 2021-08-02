namespace AdoptMe.Services.Administration
{
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Areas.Administration.Models.Shelters;
    using AdoptMe.Data.Models.Enums;

    public class AdministrationService : IAdministrationService
    {
        private readonly AdoptMeDbContext data;

        public AdministrationService(AdoptMeDbContext data)
            => this.data = data;

        public SheltersQueryViewModel ShelterRequests(int pageIndex, int pageSize)
        {
            var sheltersQuery = this.data
                .Shelters
                .Where(s => s.RegistrationStatus == RegistrationStatus.Submitted)
                .AsQueryable();

            var shelters = sheltersQuery
                .Select(x => new ShelterDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new SheltersQueryViewModel
            {
                Shelters = shelters,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalShelters = sheltersQuery.Count()
            };
        }
    }
}
