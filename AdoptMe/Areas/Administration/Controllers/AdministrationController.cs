namespace AdoptMe.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.GlobalConstants.Roles;
    using static Common.GlobalConstants.Administrator;

    [Area(AdminAreaName)]
    [Authorize(Roles = AdminRoleName)]
    public abstract class AdministrationController : Controller
    {
    }
}
