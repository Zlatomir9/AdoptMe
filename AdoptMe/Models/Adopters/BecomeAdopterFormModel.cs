namespace AdoptMe.Models.Adopters
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.AdopterRequirements;

    public class BecomeAdopterFormModel
    {
        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        public string FirstName { get; init; }

        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        public string LastName { get; init; }

        [Range(MinAge, MaxAge)]
        public int Age { get; init; }
    }
}
