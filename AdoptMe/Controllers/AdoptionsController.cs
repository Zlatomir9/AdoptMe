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
    using AdoptMe.Services.Notifications;

    using static Common.GlobalConstants.Roles;

    public class AdoptionsController : Controller
    {
        private readonly IAdoptionService adoptionService;
        private readonly IUserService userService;
        private readonly IShelterService shelterService;
        private readonly IPetService petService;
        private readonly INotificationService notificationService;

        public AdoptionsController(IAdoptionService adoptionService,
            IUserService userService,
            IShelterService shelterService,
            IPetService petService, 
            INotificationService notificationService)
        {
            this.adoptionService = adoptionService;
            this.userService = userService;
            this.shelterService = shelterService;
            this.petService = petService;
            this.notificationService = notificationService;
        }

        [Authorize(Roles = AdopterRoleName)]
        public IActionResult AdoptionApplication(int id)
        {
            var userId = this.User.GetId();

            if (User.IsAdmin() || User.IsShelter())
            {
                return BadRequest();
            }

            if (adoptionService.SentApplication(id, userId))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = AdopterRoleName)]
        public IActionResult AdoptionApplication(int id, AdoptionFormModel adoption)
        {
            if (!ModelState.IsValid)
            {
                return View(adoption);
            }

            var userId = this.User.GetId();

            this.adoptionService.CreateAdoption(
                adoption.FirstQuestion,
                adoption.SecondQuestion,
                adoption.ThirdQuestion,
                adoption.FourthQuestion,
                id,
                userId);

            var pet = this.petService.GetPetById(id);
            var shelterUserId = this.shelterService.GetShelterUserIdByPet(id);

            this.notificationService.SentAdoptionNotification(pet.Name, shelterUserId);

            return RedirectToAction("All", "Pets");
        }

        [Authorize]
        public IActionResult AdoptionRequests(AdoptionApplicationsViewModel query)
        {
            var userId = this.User.GetId();

            var queryResult = this.adoptionService.AdoptionApplications(
                query.PageIndex, userId);

            query.TotalAdoptionApplications = queryResult.TotalAdoptionApplications;
            query.Adoptions = queryResult.Adoptions;

            return View(query);
        }

        [Authorize]
        public IActionResult AdoptionApplicationDetails(AdoptionDetailsViewModel model)
        {
            var modelResult = this.adoptionService.Details(model.Id);

            if (!this.petService.AddedByShelter(modelResult.PetId, User.GetId()))
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

            this.adoptionService.ApproveAdoption(id);

            var pet = this.adoptionService.GetPetByAdoptionId(id);
            var adopter = this.adoptionService.GetAdopterByAdoptionId(id);

            this.petService.IsAdopted(pet.Id);
            this.notificationService.ApproveAdoptionNotification(pet.Name, adopter.UserId);
            this.adoptionService.DeclineAdoptionWhenPetIsDeletedOrAdopted(pet.Id);

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

            this.adoptionService.DeclineAdoption(id);

            var pet = this.adoptionService.GetPetByAdoptionId(id);
            var adopter = this.adoptionService.GetAdopterByAdoptionId(id);

            this.notificationService.DeclineAdoptionNotification(pet.Name, adopter.UserId);

            return this.RedirectToAction(nameof(AdoptionRequests));
        }
    }
}
