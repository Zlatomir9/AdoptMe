namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;

    public interface IPetService
    {
        PetsQueryServiceModel All(string species, string searchString, int pageIndex, int pageSize);

        PetDetailsServiceModel Details(int id, string Name, string Age, string Gender, string Breed,
            string Color, string MyStory, string ImageUrl, string Species, string ShelterName,
            string ShelterPhoneNumber, string ShelterEmail);

        IEnumerable<string> AllSpecies();
    }
}
