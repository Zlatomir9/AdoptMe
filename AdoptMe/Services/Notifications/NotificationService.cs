namespace AdoptMe.Services.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Notifications;

    public class NotificationService : INotificationService
    {
        private readonly AdoptMeDbContext data;

        public NotificationService(AdoptMeDbContext data)
            => this.data = data;

        public async Task<Notification> Create(string message)
        {
            var notification = new Notification
            {
                Message = message,
                CreatedOn = DateTime.UtcNow
            };

            await this.data.Notifications.AddAsync(notification);
            await this.data.SaveChangesAsync();

            return notification;
        }

        public async Task<bool> AddNotificationToUser(int notificationId, string userId)
        {
            var userNotification = new UserNotification
            {
                IsRead = false,
                NotificationId = notificationId,
                UserId = userId
            };

            if (await this.data.UserNotifications
                .AnyAsync(n => n.NotificationId == notificationId && n.UserId == userId))
            {
                return false;
            }

            await this.data.UserNotifications.AddAsync(userNotification);
            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<NotificationViewModel>> GetUserNotifications(string userId)
        {
           var notificationsQuery = this.data.UserNotifications
                          .Where(u => u.UserId == userId && u.IsRead == false)
                          .AsQueryable();

            var notifications = await notificationsQuery
                           .Select(x => new NotificationViewModel
                           {
                               Id = x.NotificationId,
                               Message = x.Notification.Message,
                               CreatedOn = x.Notification.CreatedOn
                           })
                           .OrderBy(x => x.CreatedOn)
                           .ToListAsync();

            return notifications;
        }

        public async Task ReadNotification(int notificationId, string userId)
        {
            var notification = await this.data.UserNotifications
                                   .FirstOrDefaultAsync(n => n.UserId == userId && 
                                                   n.NotificationId == notificationId);

            notification.IsRead = true;
            await this.data.SaveChangesAsync();
        }

        public async Task PetEditByAdminNotification(string petName, string userId)
        {
            var message = $"Your advertisment about {petName} has been edited by administrator.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, userId);
        }

        public async Task PetDeletedByAdminNotification(string petName, string userId)
        {
            var message = $"Your advertisment about {petName} has been deleted by administrator.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, userId);
        }

        public async Task ApproveAdoptionNotification(string petName, string userId)
        {
            var message = $"Congratulations, your application for adopting {petName} has been approved.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, userId);
        }

        public async Task DeclineAdoptionNotification(string petName, string userId)
        {
            var message = $"Your application for adopting {petName} has been declined.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, userId);
        }

        public async Task SentAdoptionNotification(string petName, string userId)
        {
            var message = $"You received new adoption application for {petName}.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, userId);
        }

        public async Task AcceptShelterRegistrationNotification(string shelterName, string shelterUserId)
        {
            var message = $"Your request for registrating as {shelterName} shelter has been approved.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, shelterUserId);
        }

        public async Task DeclineShelterRegistrationNotification(string shelterName, string shelterUserId)
        {
            string message = $"Your request for registrating as {shelterName} shelter has been declined. You can send new request.";
            var notification = await this.Create(message);

            await this.AddNotificationToUser(notification.Id, shelterUserId);
        }
    }
}