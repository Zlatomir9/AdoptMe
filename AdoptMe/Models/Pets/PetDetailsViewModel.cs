namespace AdoptMe.Models.Pets
{
    public class PetDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Age { get; set; }

        public string Gender { get; set; }

        public string Breed { get; set; }

        public string Color { get; set; }

        public string MyStory { get; set; }

        public string ImageUrl { get; set; }

        public string Species { get; init; }

        public string ShelterName { get; init; }

        public string ShelterPhoneNumber { get; init; }

        public string ShelterEmail { get; init; }
    }
}
