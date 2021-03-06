namespace AdoptMe.Controllers
{
    using System.Threading.Tasks;
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
        public async Task<IActionResult> AdoptionApplication(int id)
        {
            var userId = this.User.GetId();

            if (User.IsAdmin() || User.IsShelter())
            {
                return BadRequest();
            }

            if (await adoptionService.SentApplication(id, userId))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = AdopterRoleName)]
        public async Task<IActionResult> AdoptionApplication(int id, AdoptionFormModel adoption)
        {
            if (!ModelState.IsValid)
            {
                return View(adoption);
            }

            var userId = this.User.GetId();

            await this.adoptionService.CreateAdoption(
                adoption.FirstQuestion,
                adoption.SecondQuestion,
                adoption.ThirdQuestion,
                adoption.FourthQuestion,
                id,
                userId);

            var pet = await this.petService.GetPetById(id);
            var shelterUserId = await this.shelterService.GetShelterUserIdByPet(id);

            await this.notificationService.SentAdoptionNotification(pet.Name, shelterUserId);

            return RedirectToAction("All", "Pets");
        }

        [Authorize]
        public async Task<IActionResult> AdoptionRequests(AdoptionApplicationsViewModel query)
        {
            var userId = this.User.GetId();

            var queryResult = await this.adoptionService.AdoptionApplications(
                query.PageIndex, userId);

            query.TotalAdoptionApplications = queryResult.TotalAdoptionApplications;
            query.Adoptions = queryResult.Adoptions;

            return View(query);
        }

        [Authorize]
        public async Task<IActionResult> AdoptionApplicationDetails(AdoptionDetailsViewModel model)
        {
            var modelResult = await this.adoptionService.Details(model.Id);

            if (!await this.petService.AddedByShelter(modelResult.PetId, User.GetId()))
            {
                return BadRequest();
            }

            return View(modelResult);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ApproveAdoption(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            await this.adoptionService.ApproveAdoption(id);

            var pet = await this.adoptionService.GetPetByAdoptionId(id);
            var adopter = await this.adoptionService.GetAdopterByAdoptionId(id);

            await this.petService.IsAdopted(pet.Id);
            await this.notificationService.ApproveAdoptionNotification(pet.Name, adopter.UserId);
            await this.adoptionService.DeclineAdoptionWhenPetIsDeletedOrAdopted(pet.Id);

            return this.RedirectToAction(nameof(AdoptionRequests));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeclineAdoption(int id)
        {
            if (id == 0)
            {
                return this.NotFound();
            }

            await this.adoptionService.DeclineAdoption(id);

            var pet = await this.adoptionService.GetPetByAdoptionId(id);
            var adopter = await this.adoptionService.GetAdopterByAdoptionId(id);

            await this.notificationService.DeclineAdoptionNotification(pet.Name, adopter.UserId);

            return this.RedirectToAction(nameof(AdoptionRequests));
        }
    }
}
