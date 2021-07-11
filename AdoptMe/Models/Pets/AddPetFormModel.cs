﻿namespace AdoptMe.Models.Pets
{
    using AdoptMe.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AddPetFormModel
    {
        [Required]
        [StringLength(PetNameMaxLength, MinimumLength = PetNameMinLength)]
        public string Name { get; init; }

        [Range(PetAgeMinValue, PetAgeMaxValue)]
        public int Age { get; init; }

        public Gender Gender { get; init; }

        [Required]
        public string Breed { get; init; }

        [Required]
        public string Color { get; init; }

        [Required]
        [StringLength(PetStoryMaxLength, MinimumLength = PetStoryMinLength)]
        public string MyStory { get; init; }

        [Display(Name = "Image URL")]
        [Url]
        [Required]
        public string ImageUrl { get; init; }

        [Display(Name = "Species")]
        public int SpeciesId { get; init; }

        public IEnumerable<PetSpeciesViewModel> Species { get; set; }
    }
}