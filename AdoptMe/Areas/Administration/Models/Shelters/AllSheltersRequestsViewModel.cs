namespace AdoptMe.Areas.Administration.Models.Shelters
{
    using System.Collections.Generic;

    public class AllSheltersRequestsViewModel
    {
        public const int PageSize = 5;

        public int PageIndex { get; init; } = 1;

        public int TotalShelters { get; set; }

        public IEnumerable<ShelterDetailsViewModel> Shelters { get; set; }
    }
}
