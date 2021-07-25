namespace AdoptMe.Services.Pets
{
    public class PetDetailsServiceModel : PetServiceModel
    {
        public string Color { get; set; }

        public string MyStory { get; set; }

        public string ShelterName { get; init; }

        public string ShelterPhoneNumber { get; init; }

        public string ShelterEmail { get; init; }
    }
}
