namespace AdoptMe.Controllers
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class PetsController : Controller
    {
        private readonly AdoptMeDbContext data;

        public PetsController(AdoptMeDbContext data)
        {
            this.data = data;
        }

        public IActionResult All(AllPetsViewModel query)
        {
            var petsQuery = this.data.Pets.AsQueryable();

            if (!string.IsNullOrEmpty(query.Species))
            {
                petsQuery = petsQuery.Where(s => s.Species.Name == query.Species);
            }

            var pets = petsQuery
                .Select(x => new PetListingViewModel
                {
                    Id = x.Id,
                    Species = x.Species.ToString(),
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    Name = x.Name,
                    Age = x.Age,
                    Gender = x.Gender.ToString()
                })
                .ToList();

            var species = this.data.Species
                .Select(s => s.Name)
                .Distinct()
                .ToList();

            query.AllSpecies = species;
            query.Pets = pets;

            return View(query);
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
            if(!this.UserIsShelter())
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
                SpeciesId = pet.SpeciesId
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
