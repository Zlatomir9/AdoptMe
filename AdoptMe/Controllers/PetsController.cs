﻿namespace AdoptMe.Controllers
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
                query.SearchString);

            query.AllSpecies = species;
            query.Pets = queryResult.Pets;

            return View(query);
        }

        public IActionResult Details(int id)
        {
            var pet = this.data
                .Pets
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (pet.Id == 0)
            {
                return RedirectToAction(nameof(PetsController.All), "Pets");
            }

            var petShelter = this.data
                .Shelters
                .Where(x => x.Id == pet.ShelterId)
                .FirstOrDefault();

            var currentPet = new PetDetailsViewModel
                {
                    Name = pet.Name,
                    Breed = pet.Breed,
                    Age = pet.Age,
                    Gender = pet.Gender.ToString(),
                    Color = pet.Color,
                    MyStory = pet.MyStory,
                    Species = this.data.Species
                                .Where(x => x.Id == pet.SpeciesId)
                                .Select(x => x.Name)
                                    .ToString(),
                    Shelter = petShelter.Name,
                    ShelterEmail = petShelter.Email,
                    ShelterPhoneNumber = pet.Shelter.PhoneNumber,
                    ImageUrl = pet.ImageUrl
                };

            return View(currentPet);
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
