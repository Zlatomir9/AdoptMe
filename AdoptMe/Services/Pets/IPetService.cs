namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;

    public interface IPetService
    {
        PetsQueryServiceModel All(string species, string searchString);

        IEnumerable<string> AllSpecies();
    }
}
