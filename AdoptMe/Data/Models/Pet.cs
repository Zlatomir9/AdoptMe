namespace AdoptMe.Data.Models
{
    using AdoptMe.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;
    
    public class Pet
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(PetNameMaxLength)]
        public string Name { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public string Breed { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        [MaxLength(PetStoryMaxLength)]
        public string MyStory { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public bool IsAdopted { get; set; }

        [Required]
        public int SpeciesId { get; set; }

        public Species Species { get; init; }       
    }
}
