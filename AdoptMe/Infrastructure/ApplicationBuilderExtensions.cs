namespace AdoptMe.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;

    using static Common.GlobalConstants.Roles;
    using static Common.GlobalConstants.Administrator;
    using static Common.GlobalConstants;

    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;

            var data = scopedServices.ServiceProvider.GetService<AdoptMeDbContext>();

            data.Database.Migrate();

            SeedCategories(data);
            await SeedRoles(services);
            await SeedAdministrator(services);
            await SeedUser(services, ShelterSeeder.Username, ShelterSeeder.Email);
            await SeedUser(services, AdopterSeeder.Username, AdopterSeeder.Email);
            await SeedUser(services, AdopterSeeder.Username2, AdopterSeeder.Email2);
            await SeedAddress(services, AddressSeeder.CityName, AddressSeeder.StreetName, AddressSeeder.StreetNumber);
            await SeedShelter(services, ShelterSeeder.Username);
            await SeedAdopter(services, AdopterSeeder.Username, AdopterSeeder.FirstName, AdopterSeeder.LastName, AdopterSeeder.Age);
            await SeedAdopter(services, AdopterSeeder.Username2, AdopterSeeder.FirstName2, AdopterSeeder.LastName2, AdopterSeeder.Age2);
            await SeedPets(services);

            return app;
        }

        private static void SeedCategories(AdoptMeDbContext data)
        {
            if (data.Species.Any())
            {
                return;
            }

            data.Species.AddRange(new[]
            {
                new Species { Name = "Dog" },
                new Species { Name = "Cat" },
                new Species { Name = "Rabbit" }
            });

            data.SaveChanges();
        }

        private static async Task SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if ((await roleManager.RoleExistsAsync(AdminRoleName))
                && (await roleManager.RoleExistsAsync(ShelterRoleName))
                && (await roleManager.RoleExistsAsync(AdopterRoleName)))
            {
                return;
            }

            var adminRole = new IdentityRole { Name = AdminRoleName };
            var shelterRole = new IdentityRole { Name = ShelterRoleName };
            var adopetrRole = new IdentityRole { Name = AdopterRoleName };

            await roleManager.CreateAsync(adminRole);
            await roleManager.CreateAsync(shelterRole);
            await roleManager.CreateAsync(adopetrRole);
        }

        private static async Task SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();

            if (userManager.Users.Any(x => x.UserName == AdminUsername))
            {
                return;
            }

            var user = new User
            {
                Email = AdminEmail,
                UserName = AdminUsername
            };

            await userManager.CreateAsync(user, AdminPassword);

            await userManager.AddToRoleAsync(user, AdminRoleName);
        }

        private static async Task SeedUser(IServiceProvider services, string userName, string email)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();

            if (userManager.Users.Any(x => x.UserName == userName))
            {
                return;
            }

            var user = new User
            {
                UserName = userName,
                Email = email
            };

            await userManager.CreateAsync(user, "123456");

            if (userName == AdopterSeeder.Username || userName == AdopterSeeder.Username2)
            {
                await userManager.AddToRoleAsync(user, AdopterRoleName);
            }
            else if (userName == ShelterSeeder.Username)
            {
                await userManager.AddToRoleAsync(user, ShelterRoleName);
            }
        }

        private static async Task SeedShelter(IServiceProvider services, string userName)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var db = services.GetRequiredService<AdoptMeDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            var currShelter = await db.Shelters.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (user != null && currShelter == null)
            {
                var shelter = new Shelter
                {
                    Name = "Pet care",
                    RegistrationStatus = Data.Models.Enums.RequestStatus.Аccepted,
                    PhoneNumber = "+359 888 777 555",
                    UserId = user.Id,
                    Email = user.Email,
                    AddressId = 1
                };

                await db.Shelters.AddAsync(shelter);
                await db.SaveChangesAsync();
            }

        }

        private static async Task SeedAddress(IServiceProvider services, string cityName, string streetName, string streetNumber)
        {
            var db = services.GetRequiredService<AdoptMeDbContext>();
            var currenctCity = await db.Cities.FirstOrDefaultAsync(c => c.Name == cityName);

            if (currenctCity == null)
            {
                var city = new City
                {
                    Name = cityName,
                    Addresses = new List<Address>()
                };

                await db.Cities.AddAsync(city);
                await db.SaveChangesAsync();

                var address = new Address
                {
                    CityId = city.Id,
                    StreetName = streetName,
                    StreetNumber = streetNumber
                };

                await db.Addresses.AddAsync(address);
                await db.SaveChangesAsync();
            }
            else
            {
                var address = await db.Addresses.FirstOrDefaultAsync(a => a.StreetName == streetName
                                                            && a.StreetNumber == streetNumber
                                                            && a.CityId == currenctCity.Id);

                if (address == null)
                {
                    var newAddress = new Address
                    {
                        CityId = currenctCity.Id,
                        StreetName = streetName,
                        StreetNumber = streetNumber
                    };
                }
            }
        }

        private static async Task SeedAdopter(IServiceProvider services, string userName, string firstName, string lastName, int age)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var db = services.GetRequiredService<AdoptMeDbContext>();

            var user = await userManager.FindByNameAsync(userName);
            var currAdopter = await db.Adopters.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (user != null && currAdopter == null)
            {
                var adopter = new Adopter
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age,
                    UserId = user.Id
                };

                await userManager.AddToRoleAsync(user, AdopterRoleName);

                await db.Adopters.AddAsync(adopter);
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedPets(IServiceProvider services)
        {
            var db = services.GetRequiredService<AdoptMeDbContext>();
            var shelter = await db.Shelters.FirstOrDefaultAsync(x => x.Name == "Pet care");

            if (db.Pets.Any())
            {
                return;
            }

            var pet = new Pet
            {
                Name = "PRINCE",
                Breed = "Chow Chow",
                Age = Data.Models.Enums.Age.Young,
                Color = "Orange",
                Gender = Data.Models.Enums.Gender.Male,
                MyStory = "Wow, what a face! It should be illegal to be this cute! " +
                "Theodore is an adorable 15-month old Chow Chow who is friendly, fearless, " +
                "and more fun than a barrel of monkeys.",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/01_Chow_Chow.jpg/1200px-01_Chow_Chow.jpg",
                DateAdded = DateTime.UtcNow,
                ShelterId = shelter.Id,
                SpeciesId = 1
            };

            var pet2 = new Pet
            {
                Name = "Frank",
                Breed = "French Bulldog",
                Age = Data.Models.Enums.Age.Adult,
                Color = "Black",
                Gender = Data.Models.Enums.Gender.Male,
                MyStory = "Frank is a male French bulldog. He was rescued 5 months ago. " +
                "Due to seriously health issues of one child, the family can no longer keep him. " +
                "They say he is an amazing dog with people, kids, even strangers",
                ImageUrl = "https://www.frenchbulldogbreed.net/wp-content/uploads/2018/06/Blue-frenchie-puppy-boys-for-sale-01-1.jpg",
                DateAdded = DateTime.UtcNow,
                ShelterId = shelter.Id,
                SpeciesId = 1
            };

            var pet3 = new Pet
            {
                Name = "Gigi",
                Breed = "Domestic Shorthair",
                Age = Data.Models.Enums.Age.Adult,
                Color = "Orange Or Red",
                Gender = Data.Models.Enums.Gender.Female,
                MyStory = "Gigi is a very friendly cat. She loves playing with flowing water in the kitchen. " +
                "She is curious, intelligent and adorable. She will be a very good family company.",
                ImageUrl = "https://animalpath.org/wp-content/uploads/2021/03/Are-Domestic-Shorthair-Cats-Hypoallergenic.jpg",
                DateAdded = DateTime.UtcNow,
                ShelterId = shelter.Id,
                SpeciesId = 2
            };

            var pet4 = new Pet
            {
                Name = "Cookie",
                Breed = "French Lop",
                Age = Data.Models.Enums.Age.Adult,
                Color = "White",
                Gender = Data.Models.Enums.Gender.Male,
                MyStory = "Cookie is the most cute pet you will ever have. " +
                "He is extremely friendly and always ready to play. Look at him - he is adorable!",
                ImageUrl = "https://t2.ea.ltmcdn.com/en/images/3/2/2/caring_for_a_french_lop_rabbit_223_orig.jpg",
                DateAdded = DateTime.UtcNow,
                ShelterId = shelter.Id,
                SpeciesId = 3
            };

            await db.Pets.AddRangeAsync(pet, pet2, pet3, pet4);
            await db.SaveChangesAsync();
        }
    }
}