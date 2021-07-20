namespace AdoptMe.Models.Pets
{
    public class PetDetailsViewModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public string Breed { get; set; }

        public string Color { get; set; }

        public string MyStory { get; set; }

        public string ImageUrl { get; set; }

        public string Species { get; init; }

        public string Shelter { get; init; }

        public string ShelterPhoneNumber { get; init; }

        public string ShelterEmail { get; init; }
    }
}
