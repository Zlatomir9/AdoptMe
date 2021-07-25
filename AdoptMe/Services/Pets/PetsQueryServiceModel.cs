namespace AdoptMe.Services.Pets
{
    using System.Collections.Generic;

    public class PetsQueryServiceModel
    {
        public string SearchString { get; init; }

        public int PageIndex { get; init; }

        public int PageSize { get; init; }

        public int TotalPets { get; set; }

        public IEnumerable<PetServiceModel> Pets { get; init; }
    }
}
