namespace AdoptMe.Models.Pets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AllPetsViewModel
    {
        [Display(Name = "Search by species:")]
        public string Species { get; init; }

        [Display(Name = "Search by breed:")]
        public string SearchString { get; init; }

        public string SortOrder { get; init; }

        public int PageIndex { get; init; } = 1;

        public int TotalPets { get; set; }

        public IEnumerable<PetSpeciesModel> AllSpecies { get; set; }

        public IEnumerable<PetDetailsViewModel> Pets { get; set; }
    }
}
