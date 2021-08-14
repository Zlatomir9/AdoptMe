namespace AdoptMe.Services.Notifications
{
    using System.Collections.Generic;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Notifications;

    public interface INotificationService
    {
        IEnumerable<NotificationViewModel> GetUserNotifications(string userId);

        public Notification Create(string message);

        void ReadNotification(int notificationId, string userId);

        bool AddNotificationToUser(int notificationId, string userId);
    }
}
