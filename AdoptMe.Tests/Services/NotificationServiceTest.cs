namespace AdoptMe.Tests.Services
{
    using System;
    using System.Linq;
    using Xunit;
    using Microsoft.EntityFrameworkCore;
    using FluentAssertions;
    using AdoptMe.Data;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Data.Models;

    public class NotificationServiceTest
    {
        [Theory]
        [InlineData("message")]
        public void CreateShouldAddNotificationInDatabase(string message)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            var notification = notificationService.Create(message);

            var actualNotification = db.Notifications.FirstOrDefault();

            notification.Should().BeEquivalentTo(actualNotification);
            db.Notifications.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1, "userId")]
        [InlineData(2, "secondId")]
        public void AddNotificationToUserShoulReturnTrue(int notificationId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            var userNotification = notificationService.AddNotificationToUser(notificationId, userId);

            userNotification.Should().BeTrue();
            db.UserNotifications.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("userId", 1)]
        [InlineData("secondId", 3)]
        public void ReadNotificationShoulSetIsReadToTrue(string userId, int notificationId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            var notification = new Notification
            {
                Id = notificationId
            };

            var userNotification = new UserNotification
            {
                UserId = userId,
                NotificationId = notificationId,
                IsRead = false
            };

            db.Notifications.Add(notification);
            db.UserNotifications.Add(userNotification);
            db.SaveChanges();

            notificationService.ReadNotification(notificationId, userId);

            userNotification.IsRead.Should().BeTrue();
        }

        [Theory]
        [InlineData("petName", "userId")]
        public void PetEditByAdminNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.PetEditByAdminNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public void PetDeleteByAdminNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.PetDeletedByAdminNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public void ApproveAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.ApproveAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public void DeclineAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.DeclineAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public void SentAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.SentAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("shelterName", "userId")]
        public void AcceptShelterRegistrationNotificationShoulAddNotification(string shelterName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.AcceptShelterRegistrationNotification(shelterName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("shelterName", "userId")]
        public void DeclineShelterRegistrationNotificationShoulAddNotification(string shelterName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            notificationService.DeclineShelterRegistrationNotification(shelterName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }
    }
}
