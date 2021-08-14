﻿namespace AdoptMe.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Services.Users;

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

        public IActionResult GetNotifications()
        {
            var userId = userService.GetUserId();
            var notifications = this.notificationService.GetUserNotifications(userId);

            return Json( new { Count = notifications.Count() });
        }

        [HttpPost]
        public IActionResult ReadNotification(int notificationId)
        {
            var userId = userService.GetUserId();
            this.notificationService.ReadNotification(notificationId, userId);

            return RedirectToAction(nameof(AllNotifications));
        }

        public IActionResult AllNotifications()
        {
            var userId = userService.GetUserId();
            var notificationsQuery = this.notificationService.GetUserNotifications(userId);

            return View(notificationsQuery);
        }
    }
}