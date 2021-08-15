namespace AdoptMe.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;

    using static Common.GlobalConstants.Roles;
    using static Common.GlobalConstants.Administrator;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;

            var data = scopedServices.ServiceProvider.GetService<AdoptMeDbContext>();

            data.Database.Migrate();

            SeedCategories(data);
            SeedRoles(services);
            SeedAdministrator(services);

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

        private static void SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
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
                })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();

            if (userManager.Users.Any(x => x.UserName == AdminUsername))
            {
                return;
            }

            Task
                .Run(async () =>
                {
                    var user = new User
                    {
                        Email = AdminEmail,
                        UserName = AdminUsername
                    };

                    await userManager.CreateAsync(user, AdminPassword);

                    await userManager.AddToRoleAsync(user, AdminRoleName);
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}