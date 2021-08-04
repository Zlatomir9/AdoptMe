namespace AdoptMe.Models.Shelters
{
    using System.Collections.Generic;

    public class RegistrationRequestsViewModel
    {
        public int PageIndex { get; set; } = 1;

        public int TotalShelters { get; set; }

        public IEnumerable<ShelterDetailsViewModel> Shelters { get; set; }
    }
}
