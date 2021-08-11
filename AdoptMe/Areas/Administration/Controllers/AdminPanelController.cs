namespace AdoptMe.Areas.Administration.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Services.Administration;
    using AdoptMe.Models.Pets;
    using AdoptMe.Models.Shelters;

    public class AdminPanelController : AdministrationController
    {
        private readonly IAdministrationService administration;

        public AdminPanelController(IAdministrationService administration)
             => this.administration = administration;

        public IActionResult Index() => View();

        public IActionResult RegistrationRequests(RegistrationRequestsViewModel query)
        {
            var queryResult = this.administration.RegistrationRequests(
                query.PageIndex);

            query.TotalShelters = queryResult.TotalShelters;
            query.Shelters = queryResult.Shelters;

            return View(query);
        }

        [HttpPost]
        public IActionResult АcceptRequest(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            this.administration.AcceptRequest(id);

            return this.RedirectToAction(nameof(RegistrationRequests));
        }

        [HttpPost]
        public IActionResult DeclineRequest(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            this.administration.DeclineRequest(id);

            return this.RedirectToAction(nameof(RegistrationRequests));
        }

        public IActionResult AllPets(AllPetsViewModel query, string sortOrder)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewBag.ShelterSortParm = sortOrder == "Shelter" ? "shelter_desc" : "Shelter";
            ViewBag.CurrentSort = sortOrder;

            var queryResult = this.administration.AllPets(
                query.PageIndex, 
                query.SortOrder);

            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }
    }
}
