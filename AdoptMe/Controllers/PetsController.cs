namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using AdoptMe.Infrastructure;
    using AdoptMe.Models.Pets;
    using AdoptMe.Services.Pets;
    using AdoptMe.Services.Shelters;

    public class PetsController : Controller
    {
        private readonly IPetService pets;
        private readonly IShelterService shelters;
        private readonly IMapper mapper;

        public PetsController(IPetService pets, IShelterService shelters, IMapper mapper)
        {
            this.pets = pets;
            this.shelters = shelters;
            this.mapper = mapper;
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
            if (!this.shelters.IsShelter(this.User.GetId()))
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

            if (!this.shelters.IsShelter(userId) && !User.IsAdmin())
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
    }
}
