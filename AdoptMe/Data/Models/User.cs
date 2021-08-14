namespace AdoptMe.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public IEnumerable<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}
