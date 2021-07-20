namespace AdoptMe.Models.Pets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AdoptMe.Services.Pets;

    public class AllPetsViewModel
    {
        [Display(Name = "Search by species:")]
        public string Species { get; init; }

        [Display(Name = "Search by breed:")]
        public string SearchString { get; init; }

        public IEnumerable<string> AllSpecies { get; set; }

        public IEnumerable<PetServiceModel> Pets { get; set; }
    }
}
