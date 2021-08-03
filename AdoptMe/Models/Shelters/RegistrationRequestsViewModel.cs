namespace AdoptMe.Models.Shelters
{
    using System.Collections.Generic;

    public class RegistrationRequestsViewModel
    {
        public int PageIndex { get; init; } = 1;

        public int TotalShelters { get; set; }

        public IEnumerable<ShelterDetailsViewModel> Shelters { get; set; }
    }
}
