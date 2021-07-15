using System.Collections.Generic;

namespace AdoptMe.Models.Pets
{
    public class AllPetsViewModel
    {
        public IEnumerable<PetListingViewModel> Pets { get; set; }
    }
}
