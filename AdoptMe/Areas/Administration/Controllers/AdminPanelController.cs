namespace AdoptMe.Areas.Administration.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Services.Administration;
    using AdoptMe.Models.Pets;
    using AdoptMe.Models.Shelters;

    public class AdminPanelController : AdministrationController
    {
        private readonly IAdministrationService administration;
        private readonly AdoptMeDbContext data;

        public AdminPanelController(IAdministrationService administration, AdoptMeDbContext data)
        {
            this.administration = administration;
            this.data = data;
        }

        public IActionResult Index() => View();

        public IActionResult RegistrationRequests(RegistrationRequestsViewModel query)
        {
            var queryResult = this.administration.RegistrationRequests(
                query.PageIndex);

            query.Shelters = queryResult.Shelters;
            query.TotalShelters = queryResult.TotalShelters;

            return View(query);
        }

        [HttpPost]
        public IActionResult АcceptRequest(int id)
        {
            var shelter = this.GetShelterById(id);

            if (shelter == null)
            {
                return this.NotFound();
            }

            shelter.RegistrationStatus = RegistrationStatus.Аccepted;

            this.data.SaveChanges();

            return this.RedirectToAction("ShelterRequests");
        }

        [HttpPost]
        public IActionResult DeclineRequest(int id)
        {
            var shelter = this.GetShelterById(id);

            if (shelter == null)
            {
                return this.NotFound();
            }

            shelter.RegistrationStatus = RegistrationStatus.Declined;

            this.data.SaveChanges();

            return this.RedirectToAction("ShelterRequests");
        }

        public IActionResult AllPets(AllPetsViewModel query)
        {
            var queryResult = this.administration.AllPets(
                query.PageIndex);

            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }

        public Shelter GetShelterById(int id)
            => this.data
                .Shelters
                .Where(s => s.Id == id)
                .FirstOrDefault();
    }
}
