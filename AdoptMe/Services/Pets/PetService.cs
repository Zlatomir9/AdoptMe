namespace AdoptMe.Services.Pets
{
    using System.Linq;
    using System.Collections.Generic;
    using AdoptMe.Data;

    public class PetService : IPetService
    {
        private readonly AdoptMeDbContext data;

        public PetService(AdoptMeDbContext data)
            => this.data = data;


        public PetsQueryServiceModel All(string species, string searchString, int pageIndex, int pageSize)
        {
            var petsQuery = this.data.Pets.AsQueryable();

            if (!string.IsNullOrEmpty(species))
            {
                petsQuery = petsQuery.Where(s => s.Species.Name == species);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                petsQuery = petsQuery.Where(s => s.Breed.Contains(searchString));
            }

            var totalPets = petsQuery.Count();

            var pets = petsQuery
                .Select(x => new PetServiceModel
                {
                    Id = x.Id,
                    Species = x.Species.ToString(),
                    Breed = x.Breed,
                    ImageUrl = x.ImageUrl,
                    Name = x.Name,
                    Age = x.Age.ToString(),
                    Gender = x.Gender.ToString()
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PetsQueryServiceModel
            {
                SearchString = searchString,
                Pets = pets,
                TotalPets = totalPets,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public PetDetailsServiceModel Details(int id, string Name, string Age, string Gender, string Breed,
            string Color, string MyStory, string ImageUrl, string Species, string ShelterName,
            string ShelterPhoneNumber, string ShelterEmail)
            => this.data
                   .Pets
                   .Where(p => p.Id == id)
                   .Select(p => new PetDetailsServiceModel
                   {
                       Id = p.Id,
                       Age = p.Age.ToString(),
                       Breed = p.Breed,
                       Color = p.Color,
                       Gender = p.Gender.ToString(),
                       MyStory = p.MyStory,
                       Name = p.Name,
                       ImageUrl = p.ImageUrl,
                       Species = p.Species.ToString(),
                       ShelterName = p.Shelter.Name,
                       ShelterPhoneNumber = p.Shelter.PhoneNumber,
                       ShelterEmail = p.Shelter.Email
                   })
                   .FirstOrDefault();

        public IEnumerable<string> AllSpecies()
                => this.data.Species
                .Select(s => s.Name)
                .Distinct()
                .ToList();
    }
}
