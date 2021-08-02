namespace AdoptMe.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Data.DataConstants.Roles;
    using static Data.DataConstants.Administrator;

    [Area(AdminAreaName)]
    [Authorize(Roles = AdminRoleName)]
    public abstract class AdministrationController : Controller
    {
    }
}
