namespace AdoptMe.Models.Pets
{
    using System;

    public class PetViewModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public string Breed { get; init; }

        public string ImageUrl { get; init; }

        public string Species { get; init; }

        public string Gender { get; init; }

        public string Age { get; init; }

        public DateTime DateAdded { get; set; }
    }
}
