namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Models.Pets;

    public interface IPetService
    {
        AllPetsViewModel All(string species, string searchString, int pageIndex);

        AllPetsViewModel MyPets(int pageIndex, string sortOrder);

        PetDetailsViewModel Details(int id);

        int Add(string name, Age age, string breed, string color, Gender gender, string myStory, 
                string imageUrl, int speciesId, int shelterId);

        bool Edit(int id, string name, Age age, string breed, string color, Gender gender,
                string myStory, string imageUrl, int speciesId);

        public void Delete(int id);

        IEnumerable<PetSpeciesModel> AllSpecies();

        bool IsByShelter(int petId, int shelterId);

        public bool SpeciesExists(int speciesId);
    }
}
