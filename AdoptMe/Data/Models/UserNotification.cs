namespace AdoptMe.Data.Models
{
    public class UserNotification
    {
        public int NotificationId { get; set; }

        public Notification Notification { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public bool IsRead { get; set; }
    }
}
