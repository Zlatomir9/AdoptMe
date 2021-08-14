namespace AdoptMe.Models.Notifications
{
    using System;

    public class NotificationViewModel
    {
        public int Id { get; init; }

        public string Message { get; init; }

        public DateTime CreatedOn { get; set; }
    }
}
