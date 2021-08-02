﻿namespace AdoptMe.Areas.Administration.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Services.Administration;
    using AdoptMe.Areas.Administration.Models.Shelters;
    
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

        public IActionResult ShelterRequests(AllSheltersRequestsViewModel query)
        {
            var queryResult = this.administration.ShelterRequests(
                query.PageIndex,
                AllSheltersRequestsViewModel.PageSize);

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

        public Shelter GetShelterById(int id)
            => this.data
                .Shelters
                .Where(s => s.Id == id)
                .FirstOrDefault();
    }
}
