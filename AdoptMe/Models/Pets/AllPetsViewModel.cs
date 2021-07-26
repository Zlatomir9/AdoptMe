namespace AdoptMe.Models.Pets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AdoptMe.Services.Pets;

    public class AllPetsViewModel
    {
        public const int PageSize = 3;

        [Display(Name = "Search by species:")]
        public string Species { get; init; }

        [Display(Name = "Search by breed:")]
        public string SearchString { get; init; }

        public int PageIndex { get; init; } = 1;

        public int TotalPets { get; set; }

        public IEnumerable<PetSpeciesServiceModel> AllSpecies { get; set; }

        public IEnumerable<PetServiceModel> Pets { get; set; }
    }
}
