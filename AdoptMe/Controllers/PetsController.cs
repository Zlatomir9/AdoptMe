namespace AdoptMe.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Pets;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Services.Adoptions;

    using static Common.GlobalConstants.Roles;

    public class PetsController : Controller
    {
        private readonly IPetService petService;
        private readonly IShelterService shelterService;
        private readonly INotificationService notificationService;
        private readonly IAdoptionService adoptionService;
        private readonly IMapper mapper;

        public PetsController(IPetService petService,
            IShelterService shelterService,
            INotificationService notificationService,
            IMapper mapper, 
            IAdoptionService adoptionService)
        {
            this.petService = petService;
            this.shelterService = shelterService;
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.adoptionService = adoptionService;
        }

        public async Task<IActionResult> All(AllPetsViewModel query)
        {
            var species = await this.petService.AllSpecies();

            var queryResult = await this.petService.All(
                query.Species,
                query.SearchString,
                query.PageIndex);

            query.AllSpecies = species;
            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }

        public async Task<IActionResult> Details(PetDetailsViewModel model)
        {
           var modelResult = await this.petService.Details(model.Id);

           return View(modelResult);
        }

        [Authorize]
        [Authorize(Roles = ShelterRoleName)]
        public async Task<IActionResult> Add()
        {
            return View(new PetFormModel
            {
                AllSpecies = await this.petService.AllSpecies()
            });
        }

        [HttpPost]
        [Authorize(Roles = ShelterRoleName)]
        public async Task<IActionResult> Add(PetFormModel pet)
        {
            if (!this.petService.SpeciesExists(pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species does not exist.");
            }

            if (!ModelState.IsValid)
            {
                pet.AllSpecies = await this.petService.AllSpecies();

                return View(pet);
            }

            var shelterId = this.shelterService
                .IdByUser(this.User.GetId());

            await this.petService.Add(
                pet.Name,
                pet.Age,
                pet.Breed,
                pet.Color,
                pet.Gender,
                pet.MyStory,
                pet.ImageUrl,
                pet.SpeciesId,
                shelterId);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.User.GetId();

            if (!User.IsShelter() && !User.IsAdmin())
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            var pet = await this.petService.Details(id);

            if (pet.UserId != userId && !User.IsAdmin())
            {
                return Unauthorized();
            }

            var petForm = this.mapper.Map<PetFormModel>(pet);
            petForm.AllSpecies = await this.petService.AllSpecies();

            return View(petForm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PetFormModel pet)
        {
            if (!User.IsInRole(ShelterRoleName) 
                && !User.IsInRole(AdminRoleName))
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            if (!this.petService.AddedByShelter(id, User.GetId()) 
                && !User.IsInRole(AdminRoleName))
            {
                return BadRequest();
            }

            if (!this.petService.SpeciesExists(pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species is not supported.");
            }

            if (!ModelState.IsValid)
            {
                pet.AllSpecies = await this.petService.AllSpecies();

                return View(pet);
            }

            await this.petService.Edit(
                id,
                pet.Name,
                pet.Age,
                pet.Breed,
                pet.Color,
                pet.Gender,
                pet.MyStory,
                pet.ImageUrl,
                pet.SpeciesId);

            if (User.IsInRole(AdminRoleName))
            {
               var shelterUserId = shelterService.GetShelterUserIdByPet(id);
               this.notificationService.PetEditByAdminNotification(pet.Name, shelterUserId);
            }

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = this.petService.GetPetById(id);

            if (!this.petService.AddedByShelter(id, User.GetId()) 
                && !User.IsInRole(AdminRoleName))
            {
                return BadRequest();
            }

            if (pet == null)
            {
                return BadRequest();
            }

            await this.petService.Delete(id);

            this.adoptionService.DeclineAdoptionWhenPetIsDeletedOrAdopted(id);

            if (User.IsInRole(AdminRoleName))
            {
                var shelterUserId = shelterService.GetShelterUserIdByPet(id);
                this.notificationService.PetDeletedByAdminNotification(pet.Name, shelterUserId);
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = ShelterRoleName)]
        public IActionResult MyPets(AllPetsViewModel query, string sortOrder)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.CurrentSort = sortOrder;

            var userId = this.User.GetId();

            var queryResult = this.petService.MyPets(
                query.PageIndex,
                query.SortOrder,
                userId);

            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }
    }
}