namespace AdoptMe.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Services.Users;
    using AdoptMe.Infrastructure;

    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;

        public NotificationsController(INotificationService notifications, IUserService userService)
        {
            this.notificationService = notifications;
            this.userService = userService;
        }

        public async Task<IActionResult> GetNotifications()
        {
            var userId = this.User.GetId();
            var notifications = await this.notificationService.GetUserNotifications(userId);

            return Json( new { Count = notifications.Count() });
        }

        [HttpPost]
        public async Task<IActionResult> ReadNotification(int notificationId)
        {
            var userId = this.User.GetId();
            await this.notificationService.ReadNotification(notificationId, userId);

            return RedirectToAction(nameof(AllNotifications));
        }

        public async Task<IActionResult> AllNotifications()
        {
            var userId = this.User.GetId();
            var notificationsQuery = await this.notificationService.GetUserNotifications(userId);

            return View(notificationsQuery);
        }
    }
}