namespace AdoptMe.Models.Pets
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.AnswerRequirements;
    using static Common.GlobalConstants;

    public class AdoptionFormModel
    {
        public int Id { get; init; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = AdoptionApplicationQuestions.FirstQuestion)]
        public string FirstQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = AdoptionApplicationQuestions.SecondQuestion)]
        public string SecondQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = AdoptionApplicationQuestions.ThirdQuestion)]
        public string ThirdQuestion { get; set; }

        [Required]
        [MinLength(AnswerMinLength, ErrorMessage = AnswerLengthErrorMessage)]
        [Display(Name = AdoptionApplicationQuestions.FourthQuestion)]
        public string FourthQuestion { get; set; }
    }
}
