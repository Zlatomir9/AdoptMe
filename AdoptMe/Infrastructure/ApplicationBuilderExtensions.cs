namespace AdoptMe.Infrastructure
{
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<AdoptMeDbContext>();

            data.Database.Migrate();

            SeedCategories(data);

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
    }
}
