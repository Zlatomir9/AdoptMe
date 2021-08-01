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

    using static Data.DataConstants.Roles;
    using static Data.DataConstants.Administrator;

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
                        && (await roleManager.RoleExistsAsync(ShelterRoleName)))
                    {
                        return;
                    }

                    var adminRole = new IdentityRole { Name = AdminRoleName };
                    var shelterRole = new IdentityRole { Name = ShelterRoleName };

                    await roleManager.CreateAsync(adminRole);
                    await roleManager.CreateAsync(shelterRole);
                })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();


            Task
                .Run(async () =>
                {
                    var user = new IdentityUser
                    {
                        Email = adminEmail,
                        UserName = adminUsername
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, AdminRoleName);
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}