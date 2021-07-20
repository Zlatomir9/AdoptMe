namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;

    public class PetsQueryServiceModel
    {
        public string SearchString { get; init; }

        public IEnumerable<PetServiceModel> Pets { get; init; }
    }
}
