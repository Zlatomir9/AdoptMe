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

    public class PetsController : Controller
    {
        private readonly IPetService pets;
        private readonly IShelterService shelters;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public PetsController(IPetService pets, IShelterService shelters, IMapper mapper, IUserService userService)
        {
            this.pets = pets;
            this.shelters = shelters;
            this.mapper = mapper;
            this.userService = userService;
        }

        public IActionResult All(AllPetsViewModel query)
        {
            var species = this.pets.AllSpecies();

            var queryResult = this.pets.All(
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
           var modelResult = this.pets.Details(model.Id);

           return View(modelResult);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!User.IsShelter())
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            return View(new PetFormModel
            {
                AllSpecies = this.pets.AllSpecies()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(PetFormModel pet)
        {
            var shelterId = this.shelters.IdByUser(this.User.GetId());

            if (shelterId == 0)
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            if (!this.pets.SpeciesExists(pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species does not exist.");
            }

            if (!ModelState.IsValid)
            {
                pet.AllSpecies = this.pets.AllSpecies();

                return View(pet);
            }

            this.pets.Add(
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

            var pet = this.pets.Details(id);

            if (pet.UserId != userId && !User.IsAdmin())
            {
                return Unauthorized();
            }

            var petForm = this.mapper.Map<PetFormModel>(pet);
            petForm.AllSpecies = this.pets.AllSpecies();

            return View(petForm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, PetFormModel pet)
        {
            var shelterId = this.shelters.IdByUser(this.User.GetId());

            if (shelterId == 0)
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            if (!this.pets.SpeciesExists(pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species is not supported.");
            }

            if (!ModelState.IsValid)
            {
                pet.AllSpecies = this.pets.AllSpecies();

                return View(pet);
            }

            if (!this.pets.IsByShelter(id, shelterId))
            {
                return BadRequest();
            }

            this.pets.Edit(
                id,
                pet.Name,
                pet.Age,
                pet.Breed,
                pet.Color,
                pet.Gender,
                pet.MyStory,
                pet.ImageUrl,
                pet.SpeciesId);

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var shelterId = this.shelters.IdByUser(this.User.GetId());

            if (shelterId == 0)
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            if (!this.pets.IsByShelter(id, shelterId))
            {
                return BadRequest();
            }

            this.pets.Delete(id);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult MyPets(AllPetsViewModel query, string sortOrder)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.CurrentSort = sortOrder;

            var queryResult = this.pets.MyPets(
                query.PageIndex,
                query.SortOrder);

            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }
    }
}