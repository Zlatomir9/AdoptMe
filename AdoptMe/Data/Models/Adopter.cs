namespace AdoptMe.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.AdopterRequirements;

    public class Adopter
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(FirstLastNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(FirstLastNameMaxLength)]
        public string LastName { get; set; }

        public int Age { get; set; }

        [Required]
        public string UserId { get; set; }

        public IEnumerable<AdoptionApplication> AdoptionApplications { get; init; } = new List<AdoptionApplication>();
    }
}
