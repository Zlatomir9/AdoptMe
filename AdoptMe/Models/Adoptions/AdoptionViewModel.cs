namespace AdoptMe.Models.Adoptions
{
    using System;

    public class AdoptionViewModel
    {
        public int Id { get; init; }

        public string AdopterFullName { get; init; }

        public string PetName { get; set; }

        public DateTime SubmittedOn { get; init; }
    }
}
