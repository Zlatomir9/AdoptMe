namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using AdoptMe.Models.Pets;
    using AdoptMe.Infrastructure;
    using AdoptMe.Services.Adoptions;
    using AdoptMe.Models.Adoptions;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Services.Pets;

    public class AdoptionsController : Controller
    {
        private readonly IAdoptionService adoptions;
        private readonly IUserService users;
        private readonly IShelterService shelters;
        private readonly IPetService pets;

        public AdoptionsController(IAdoptionService adoptions, IUserService users, 
            IShelterService shelters, IPetService pets)
        {
            this.adoptions = adoptions;
            this.users = users;
            this.shelters = shelters;
            this.pets = pets;
        }

        [Authorize]
        public IActionResult AdoptionApplication(int id)
        {
            if (User.IsAdmin() || User.IsShelter())
            {
                return BadRequest();
            }

            if (adoptions.SentApplication(id))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AdoptionApplication(int id, AdoptionFormModel adoption)
        {
            if (!ModelState.IsValid)
            {
                return View(adoption);
            }

            this.adoptions.CreateAdoption(
                adoption.FirstName,
                adoption.LastName,
                adoption.Age,
                adoption.FirstQuestion,
                adoption.SecondQuestion,
                adoption.ThirdQuestion,
                adoption.FourthQuestion,
                id);

            return RedirectToAction("All", "Pets");
        }

        [Authorize]
        public IActionResult AdoptionRequests(AdoptionApplicationsViewModel query)
        {
            var queryResult = this.adoptions.AdoptionApplications(
                query.PageIndex);

            query.TotalAdoptionApplications = queryResult.TotalAdoptionApplications;
            query.Adoptions = queryResult.Adoptions;

            return View(query);
        }

        [Authorize]
        public IActionResult AdoptionApplicationDetails(AdoptionDetailsViewModel model)
        {
            var modelResult = this.adoptions.Details(model.Id);

            if (!this.pets.AddedByShelter(modelResult.PetId, User.GetId()))
            {
                return BadRequest();
            }

            return View(modelResult);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ApproveAdoption(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            this.adoptions.ApproveAdoption(id);

            return this.RedirectToAction(nameof(AdoptionRequests));
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeclineAdoption(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            this.adoptions.DeclineAdoption(id);

            return this.RedirectToAction(nameof(AdoptionRequests));
        }
    }
}
