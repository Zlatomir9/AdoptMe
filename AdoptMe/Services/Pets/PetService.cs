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
                    Age = x.Age,
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

        public IEnumerable<string> AllSpecies()
                => this.data.Species
                .Select(s => s.Name)
                .Distinct()
                .ToList();
    }
}
