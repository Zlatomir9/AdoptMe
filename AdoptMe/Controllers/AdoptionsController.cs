namespace AdoptMe.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AdoptMe.Models.Pets;
    using AdoptMe.Infrastructure;
    using AdoptMe.Services.Adoptions;

    public class AdoptionsController : Controller
    {
        private readonly IAdoptionService adoptions;
        
        public AdoptionsController(IAdoptionService adoptions)
            => this.adoptions = adoptions;

        [Authorize]
        public IActionResult AdoptionApplication(int id)
        {
            if (User.IsAdmin() || User.IsShelter())
            {
                return BadRequest();
            }

            if (adoptions.SentApplication(id))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AdoptionApplication(int id, AdoptionFormModel adoption)
        {
            if (!ModelState.IsValid)
            {
                return View(adoption);
            }

            this.adoptions.CreateAdoption(
                adoption.FirstName,
                adoption.LastName,
                adoption.Age,
                adoption.FirstQuestion,
                adoption.SecondQuestion,
                adoption.ThirdQuestion,
                adoption.FourthQuestion,
                id);

            return RedirectToAction("All", "Pets");
        }
    }
}
