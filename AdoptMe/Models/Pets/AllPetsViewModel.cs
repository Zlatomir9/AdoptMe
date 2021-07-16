namespace AdoptMe.Models.Pets
{
    using System.Collections.Generic;

    public class AllPetsViewModel
    {
        public string Species { get; init; }

        public IEnumerable<string> AllSpecies { get; set; }

        public IEnumerable<PetListingViewModel> Pets { get; set; }
    }
}
