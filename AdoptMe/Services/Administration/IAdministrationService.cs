namespace AdoptMe.Services.Administration
{
    using AdoptMe.Areas.Administration.Models.Shelters;

    public interface IAdministrationService
    {
        SheltersQueryViewModel ShelterRequests(int pageIndex, int pageSize);
    }
}
