namespace AdoptMe.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Pets;
    using AdoptMe.Services.Shelters;

    public class PetsController : Controller
    {
        private readonly IPetService pets;
        private readonly IShelterService shelters;
        private readonly AdoptMeDbContext data;

        public PetsController(AdoptMeDbContext data, IPetService pets, IShelterService shelters)
        {
            this.data = data;
            this.pets = pets;
            this.shelters = shelters;
        }

        public IActionResult All(AllPetsViewModel query)
        {
            var species = this.pets.AllSpecies();

            var queryResult = this.pets.All(
                query.Species,
                query.SearchString,
                query.PageIndex,
                AllPetsViewModel.PageSize);

            query.AllSpecies = species;
            query.TotalPets = queryResult.TotalPets;
            query.Pets = queryResult.Pets;

            return View(query);
        }

        public IActionResult Details(PetDetailsServiceModel model)
        {
           var modelResult = this.pets.Details(model.Id);

           return View(modelResult);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.shelters.IsShelter(this.User.GetId()))
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            return View(new PetFormModel
            {
                Species = this.pets.AllSpecies()
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

            if (!this.data.Species.Any(s => s.Id == pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species does not exist.");
            }

            if (!ModelState.IsValid)
            {
                pet.Species = this.pets.AllSpecies();

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

            if (!this.shelters.IsShelter(userId))
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            var pet = this.pets.Details(id);

            if (pet.UserId != userId)
            {
                return Unauthorized();
            }

            return View(new PetFormModel
            {
                Breed = pet.Breed,
                Name = pet.Name,
                Age = Enum.Parse<Age>(pet.Age),
                Color = pet.Color,
                Gender = Enum.Parse<Gender>(pet.Gender),
                SpeciesId = pet.SpeciesId,
                ImageUrl = pet.ImageUrl,
                MyStory = pet.MyStory,
                Species = this.pets.AllSpecies()
            });
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
                pet.Species = this.pets.AllSpecies();

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
    }
}
