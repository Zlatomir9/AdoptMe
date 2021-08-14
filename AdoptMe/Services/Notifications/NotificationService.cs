namespace AdoptMe.Services.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Notifications;

    public class NotificationService : INotificationService
    {
        private readonly AdoptMeDbContext data;

        public NotificationService(AdoptMeDbContext data)
        {
            this.data = data;
        }

        public Notification Create(string message)
        {
            var notification = new Notification
            {
                Message = message,
                CreatedOn = DateTime.UtcNow
            };

            this.data.Notifications.Add(notification);
            this.data.SaveChanges();

            return notification;
        }

        public bool AddNotificationToUser(int notificationId, string userId)
        {
            var userNotification = new UserNotification
            {
                IsRead = false,
                NotificationId = notificationId,
                UserId = userId
            };

            if (this.data.UserNotifications.Any(n => n.NotificationId == notificationId && n.UserId == userId))
            {
                return false;
            }

            this.data.UserNotifications.Add(userNotification);
            this.data.SaveChangesAsync();

            return true;
        }

        public IEnumerable<NotificationViewModel> GetUserNotifications(string userId)
        {
           var notificationsQuery = this.data.UserNotifications
                          .Where(u => u.UserId == userId && u.IsRead == false)
                          .AsQueryable();

            var notifications = notificationsQuery
                           .Select(x => new NotificationViewModel
                           {
                               Id = x.NotificationId,
                               Message = x.Notification.Message,
                               CreatedOn = x.Notification.CreatedOn
                           })
                           .OrderBy(x => x.CreatedOn)
                           .ToList();

            return notifications;
        }

        public void ReadNotification(int notificationId, string userId)
        {
            var notification = this.data.UserNotifications
                                   .FirstOrDefault(n => n.UserId == userId && 
                                                   n.NotificationId == notificationId);

            notification.IsRead = true;
            this.data.SaveChanges();
        }
    }
}
