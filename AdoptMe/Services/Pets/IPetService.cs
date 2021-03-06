namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdoptMe.Data.Models;
    using AdoptMe.Data.Models.Enums;
    using AdoptMe.Models.Pets;

    public interface IPetService
    {
        Task<AllPetsViewModel> All(string species, string searchString, int pageIndex);

        Task<AllPetsViewModel> MyPets(int pageIndex, string sortOrder, string userId);

        Task<PetDetailsViewModel> Details(int id);

        Task<int> Add(string name, Age age, string breed, string color, Gender gender, string myStory, 
                string imageUrl, int speciesId, int shelterId);

        Task<bool> Edit(int id, string name, Age age, string breed, string color, Gender gender,
                string myStory, string imageUrl, int speciesId);

        Task Delete(int id);

        Task IsAdopted(int id);

        Task<IEnumerable<PetSpeciesModel>> AllSpecies();

        Task<bool> AddedByShelter(int petId, string userId);

        public bool SpeciesExists(int speciesId);

        Task<Pet> GetPetById(int id);
    }
}
