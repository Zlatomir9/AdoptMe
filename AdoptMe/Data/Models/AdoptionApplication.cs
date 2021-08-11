namespace AdoptMe.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AdoptMe.Data.Models.Enums;

    public class AdoptionApplication
    {
        public int Id { get; set; }

        [Required]
        public string FirstAnswer { get; set; }

        [Required]
        public string SecondAnswer { get; set; }

        [Required]
        public string ThirdAnswer { get; set; }

        [Required]
        public string FourthAnswer { get; set; }

        public RequestStatus RequestStatus { get; set; }

        public DateTime SubmittedOn { get; set; }

        public int PetId { get; set; }

        public Pet Pet { get; set; }

        public int AdopterId { get; set; }

        public Adopter Adopter { get; set; }
    }
}
