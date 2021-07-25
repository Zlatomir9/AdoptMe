namespace AdoptMe.Controllers
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Pets;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class PetsController : Controller
    {
        private readonly IPetService pets;
        private readonly AdoptMeDbContext data;

        public PetsController(AdoptMeDbContext data, IPetService pets)
        {
            this.data = data;
            this.pets = pets;
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
           var modelResult = this.pets.Details(
                      model.Id,
                      model.Name,
                      model.Age,
                      model.Gender,
                      model.Breed,
                      model.Color,
                      model.MyStory,
                      model.ImageUrl,
                      model.Species,
                      model.ShelterName,
                      model.ShelterPhoneNumber,
                      model.ShelterEmail);

            return View(modelResult);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.UserIsShelter())
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            return View(new AddPetFormModel
            {
                Species = this.GetPetSpecies()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(AddPetFormModel pet)
        {
            var shelterId = this.data.Shelters
                .Where(s => s.UserId == this.User.GetId())
                .Select(s => s.Id)
                .FirstOrDefault();

            if (shelterId == 0)
            {
                return RedirectToAction(nameof(SheltersController.Create), "Dealers");
            }

            if (!this.UserIsShelter())
            {
                return RedirectToAction(nameof(SheltersController.Create), "Shelters");
            }

            if (!this.data.Species.Any(s => s.Id == pet.SpeciesId))
            {
                this.ModelState.AddModelError(nameof(pet.SpeciesId), "Species does not exist.");
            }

            if (!ModelState.IsValid)
            {
                pet.Species = this.GetPetSpecies();

                return View(pet);
            }

            var currentPet = new Pet
            {
                Name = pet.Name,
                Age = pet.Age,
                Breed = pet.Breed,
                Color = pet.Color,
                Gender = pet.Gender,
                MyStory = pet.MyStory,
                ImageUrl = pet.ImageUrl,
                SpeciesId = pet.SpeciesId,
                ShelterId = shelterId
            };

            this.data.Pets.Add(currentPet);
            this.data.SaveChanges();

            return RedirectToAction("All", "Pets");
        }

        private bool UserIsShelter()
            => this.data
                .Shelters
                .Any(s => s.UserId == this.User.GetId());

        private IEnumerable<PetSpeciesViewModel> GetPetSpecies()
            => this.data.Species
                        .Select(s => new PetSpeciesViewModel
                        {
                            Id = s.Id,
                            Name = s.Name
                        })
                        .ToList();
    }
}
