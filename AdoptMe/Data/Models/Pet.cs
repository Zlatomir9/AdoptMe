namespace AdoptMe.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AdoptMe.Data.Models.Enums;

    using static DataConstants.PetRequirements;
    
    public class Pet
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public Age Age { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public string Breed { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        [MaxLength(StoryMaxLength)]
        public string MyStory { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public bool IsAdopted { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateAdded { get; set; }

        public int SpeciesId { get; set; }

        public Species Species { get; init; }

        public int ShelterId { get; init; }

        public Shelter Shelter { get; init; }

        public IEnumerable<AdoptionApplication> AdoptionApplications { get; init; } = new List<AdoptionApplication>();
    }
}
