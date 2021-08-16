namespace AdoptMe.Models.Pets
{
    using AdoptMe.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.PetRequirements;

    public class PetFormModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; init; }

        public Age Age { get; init; }

        public Gender Gender { get; init; }

        [Required]
        public string Breed { get; init; }

        [Required]
        public string Color { get; init; }

        [Required]
        [StringLength(StoryMaxLength, MinimumLength = StoryMinLength)]
        public string MyStory { get; init; }

        [Display(Name = "Image URL")]
        [Url]
        [Required]
        public string ImageUrl { get; init; }

        [Display(Name = "Species")]
        public int SpeciesId { get; init; }

        public IEnumerable<PetSpeciesModel> AllSpecies { get; set; }
    }
}