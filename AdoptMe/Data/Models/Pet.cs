namespace AdoptMe.Data.Models
{
    using AdoptMe.Data.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;

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

        public DateTime DateAdded { get; set; }

        public int SpeciesId { get; set; }

        public Species Species { get; init; }

        public int ShelterId { get; init; }

        public Shelter Shelter { get; init; }
    }
}
