namespace AdoptMe.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
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
        public async Task CreateShouldAddNotificationInDatabase(string message)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            var notification = await notificationService.Create(message);

            var actualNotification = await db.Notifications.FirstOrDefaultAsync();

            notification.Should().BeEquivalentTo(actualNotification);
            db.Notifications.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1, "userId")]
        [InlineData(2, "secondId")]
        public async Task AddNotificationToUserShoulReturnTrue(int notificationId, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            var userNotification = await notificationService.AddNotificationToUser(notificationId, userId);

            userNotification.Should().BeTrue();
            db.UserNotifications.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("userId", 1)]
        [InlineData("secondId", 3)]
        public async Task ReadNotificationShoulSetIsReadToTrue(string userId, int notificationId)
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

            await db.Notifications.AddAsync(notification);
            await db.UserNotifications.AddAsync(userNotification);
            await db.SaveChangesAsync();

            await notificationService.ReadNotification(notificationId, userId);

            userNotification.IsRead.Should().BeTrue();
        }

        [Theory]
        [InlineData("petName", "userId")]
        public async Task PetEditByAdminNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.PetEditByAdminNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public async Task PetDeleteByAdminNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.PetDeletedByAdminNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public async Task ApproveAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.ApproveAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public async Task DeclineAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.DeclineAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("petName", "userId")]
        public async Task SentAdoptionNotificationShoulAddNotification(string petName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.SentAdoptionNotification(petName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("shelterName", "userId")]
        public async Task AcceptShelterRegistrationNotificationShoulAddNotification(string shelterName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.AcceptShelterRegistrationNotification(shelterName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }

        [Theory]
        [InlineData("shelterName", "userId")]
        public async Task DeclineShelterRegistrationNotificationShoulAddNotification(string shelterName, string userId)
        {
            var guid = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AdoptMeDbContext>()
                        .UseInMemoryDatabase(guid)
                        .Options;

            var db = new AdoptMeDbContext(options);

            var notificationService = new NotificationService(db);

            await notificationService.DeclineShelterRegistrationNotification(shelterName, userId);

            db.UserNotifications.Should().HaveCount(1);
            db.UserNotifications.Should().Contain(x => x.UserId == userId);
        }
    }
}
