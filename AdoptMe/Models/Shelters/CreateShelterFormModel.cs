namespace AdoptMe.Models.Shelters
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
        public string CityName { get; set; }

        [Required]
        public string StreetName { get; set; }

        public string StreetNumber { get; set; }
    }
}