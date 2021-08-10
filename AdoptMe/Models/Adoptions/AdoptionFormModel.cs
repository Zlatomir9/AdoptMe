namespace AdoptMe.Models.Pets
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.AdopterRequirements;
    using static Data.DataConstants.AnswerRequirements;

    public class AdoptionFormModel
    {
        public int Id { get; init; }

        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        public string FirstName { get; init; }

        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        public string LastName { get; init; }

        [Range(MinAge, MaxAge)]
        public int Age { get; init; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = "Why do you want to adopt a pet?")]
        public string FirstQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = "Do you live in a house with yard or in flat?")]
        public string SecondQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = "Have you had a pet before?")]
        public string ThirdQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = "Do you currently have any other pets?")]
        public string FourthQuestion { get; set; }
    }
}
