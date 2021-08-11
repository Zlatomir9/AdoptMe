namespace AdoptMe.Models.Adoptions
{
    using System.Collections.Generic;

    public class AdoptionApplicationsViewModel
    {
        public int PageIndex { get; set; } = 1;

        public int TotalAdoptionApplications { get; set; }

        public IEnumerable<AdoptionViewModel> Adoptions { get; set; }
    }
}
