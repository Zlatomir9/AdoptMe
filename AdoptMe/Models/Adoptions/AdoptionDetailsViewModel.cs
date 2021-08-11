namespace AdoptMe.Models.Adoptions
{
    using System.ComponentModel.DataAnnotations;

    using static Common.GlobalConstants.AdoptionApplicationQuestions;

    public class AdoptionDetailsViewModel : AdoptionViewModel
    {
        public int AdopterAge { get; init; }

        [Display(Name = FirstQuestion)]
        public string FirstAnswer { get; init; }

        [Display(Name = SecondQuestion)]
        public string SecondAnswer { get; init; }

        [Display(Name = ThirdQuestion)]
        public string ThirdAnswer { get; init; }

        [Display(Name = FourthQuestion)]
        public string FourthAnswer { get; init; }
    }
}
