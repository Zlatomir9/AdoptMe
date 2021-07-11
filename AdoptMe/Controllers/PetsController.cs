namespace AdoptMe.Controllers
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Pets;
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

        public IActionResult Add() => View(new AddPetFormModel 
        {
            Species = this.GetPetSpecies()
        });

        [HttpPost]
        public IActionResult Add(AddPetFormModel pet)
        {
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

            return RedirectToAction("Index", "Home");
        }

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
