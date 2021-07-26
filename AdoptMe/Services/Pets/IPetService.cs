namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;
    using AdoptMe.Data.Models.Enums;

    public interface IPetService
    {
        PetsQueryServiceModel All(string species, string searchString, int pageIndex, int pageSize);

        PetDetailsServiceModel Details(int id);

        int Add(string name, Age age, string breed, string color, Gender gender, string myStory, 
                string imageUrl, int speciesId, int shelterId);

        bool Edit(int id, string name, Age age, string breed, string color, Gender gender,
                string myStory, string imageUrl, int speciesId);

        IEnumerable<PetSpeciesServiceModel> AllSpecies();

        bool IsByShelter(int petId, int shelterId);

        public bool SpeciesExists(int speciesId);

        IEnumerable<PetServiceModel> ByUser(string userId);
    }
}
