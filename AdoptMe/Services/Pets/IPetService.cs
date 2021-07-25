namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;

    public interface IPetService
    {
        PetsQueryServiceModel All(string species, string searchString, int pageIndex, int pageSize);

        IEnumerable<string> AllSpecies();
    }
}
