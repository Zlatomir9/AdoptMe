namespace AdoptMe.Services.Notifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Notifications;

    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewModel>> GetUserNotifications(string userId);

        Task<Notification> Create(string message);

        Task ReadNotification(int notificationId, string userId);

        Task<bool> AddNotificationToUser(int notificationId, string userId);

        Task PetEditByAdminNotification(string petName, string userId);

        Task PetDeletedByAdminNotification(string petName, string userId);

        Task ApproveAdoptionNotification(string petName, string userId);

        Task DeclineAdoptionNotification(string petName, string userId);

        Task SentAdoptionNotification(string petName, string userId);

        Task AcceptShelterRegistrationNotification(string shelterName, string shelterUserId);

        Task DeclineShelterRegistrationNotification(string shelterName, string shelterUserId);
    }
}
