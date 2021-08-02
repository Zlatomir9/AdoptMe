namespace AdoptMe.Areas.Administration.Models.Shelters
{
    using System.Collections.Generic;

    public class SheltersQueryViewModel
    {
        public int PageIndex { get; init; }

        public int PageSize { get; init; }

        public int TotalShelters { get; init; }

        public IEnumerable<ShelterDetailsViewModel> Shelters { get; set; }
    }
}
