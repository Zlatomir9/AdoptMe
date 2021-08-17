namespace AdoptMe
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using AdoptMe.Data;
    using AdoptMe.Data.Models;
    using AdoptMe.Infrastructure;
    using AdoptMe.Services.Administration;
    using AdoptMe.Services.Adoptions;
    using AdoptMe.Services.Notifications;
    using AdoptMe.Services.Pets;
    using AdoptMe.Services.Shelters;
    using AdoptMe.Services.Users;
    using AdoptMe.Services.Statistics;
    using AdoptMe.Services.Adopters;

    public class Startup
    {
        public Startup(IConfiguration configuration)
            => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<AdoptMeDbContext>(options => 
                    options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDefaultIdentity<User>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AdoptMeDbContext>();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

            services.AddTransient<IPetService, PetService>()
                    .AddTransient<IShelterService, ShelterService>()
                    .AddTransient<IAdministrationService, AdministrationService>()
                    .AddTransient<IUserService, UserService>()
                    .AddTransient<IAdoptionService, AdoptionService>()
                    .AddTransient<INotificationService, NotificationService>()
                    .AddTransient<IStatisticsService, StatisticsService>()
                    .AddTransient<IAdopterService, AdopterService>();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMilliseconds(1);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PrepareDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapDefaultAreaRoute();
                   endpoints.MapDefaultControllerRoute();
                   endpoints.MapRazorPages();
               });
        }
    }
}
