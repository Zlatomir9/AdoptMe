namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adopters;
    using AdoptMe.Services.Adopters;

    using static Common.GlobalConstants.Roles;

    public class AdoptersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IAdopterService adopterService;

        public AdoptersController(UserManager<User> userManager, IAdopterService adopterService)
        {
            this.userManager = userManager;
            this.adopterService = adopterService;
        }

        [Authorize]
        public IActionResult Become()
        {
            if (User.IsInRole(ShelterRoleName)
                || User.IsInRole(AdopterRoleName)
                || User.IsInRole(AdminRoleName))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Become(BecomeAdopterFormModel adopter)
        {
            if (!ModelState.IsValid)
            {
                return View(adopter);
            }

            var userId = this.userManager.GetUserId(this.User);

            this.adopterService.Create(
                adopter.FirstName,
                adopter.LastName,
                adopter.Age,
                userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
