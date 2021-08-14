namespace AdoptMe.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}
