namespace AdoptMe.Controllers
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Pets;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Notifications;

    using static Common.GlobalConstants.Roles;
    using AdoptMe.Services.Adoptions;

    public class PetsController : Controller
    {
        private readonly IPetService petService;
        private readonly IShelterService shelterService;
        private readonly IUserService userService;
        private readonly INotificationService notificationService;
        private readonly IAdoptionService adoptionService;
        private readonly IMapper mapper;

        public PetsController(IPetService petService,
            IShelterService shelterService,
            IUserService userService,
            INotificationService notificationService,
            IMapper mapper, 
            IAdoptionService adoptionService)
        {
            this.petService = petService;
            this.shelterService = shelterService;
            this.userService = userService;
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.adoptionService = adoptionService;
        }

        public IActionResult All(AllPetsViewModel query)
        {
            var species = this.petService.AllSpecies();

            var queryResult = this.petService.All(
                query.Species,
                query.SearchString,
                query.PageIndex);

            query.AllSpecies = species;
            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }

        public IActionResult Details(PetDetailsViewModel model)
        {
           var modelResult = this.petService.
                Details(model.Id);

           return View(modelResult);
        }

        [Authorize]
        [Authorize(Roles = ShelterRoleName)]
        public IActionResult Add()
        {
            return View(new PetFormModel
            {
                AllSpecies = this.petService.AllSpecies()
            });
        }

        [HttpPost]
        [Authorize(Roles = ShelterRoleName)]
        public IActionResult Add(PetFormModel pet)
        {
            if (!this.petService.SpeciesExists(pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species does not exist.");
            }

            if (!ModelState.IsValid)
            {
                pet.AllSpecies = this.petService.AllSpecies();

                return View(pet);
            }

            var shelterId = this.shelterService
                .IdByUser(this.userService.GetUserId());

            this.petService.Add(
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
        public IActionResult Edit(int id)
        {
            var userId = this.User.GetId();

            if (!User.IsShelter() && !User.IsAdmin())
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            var pet = this.petService.Details(id);

            if (pet.UserId != userId && !User.IsAdmin())
            {
                return Unauthorized();
            }

            var petForm = this.mapper.Map<PetFormModel>(pet);
            petForm.AllSpecies = this.petService.AllSpecies();

            return View(petForm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, PetFormModel pet)
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
                pet.AllSpecies = this.petService.AllSpecies();

                return View(pet);
            }

            this.petService.Edit(
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
        public IActionResult Delete(int id)
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

            this.petService.Delete(id);

            this.adoptionService.DeclineAdoptionWhenPetIsDeleted(id);

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

            var queryResult = this.petService.MyPets(
                query.PageIndex,
                query.SortOrder);

            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }
    }
}