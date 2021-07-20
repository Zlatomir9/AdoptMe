﻿namespace AdoptMe.Models.Shelters
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.ShelterRequirements;

    public class CreateShelterFormModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(PhoneMaxLength, MinimumLength = PhoneMinLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }
    }
}