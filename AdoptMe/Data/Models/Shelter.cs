﻿namespace AdoptMe.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.ShelterRequirements;

    public class Shelter
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(PhoneMaxLength)]
        public string PhoneNumber { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public string UserId { get; set; }

        public IEnumerable<Pet> Pets { get; init; } = new List<Pet>();
    }
}