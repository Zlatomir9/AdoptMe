namespace AdoptMe.Data
{
    using AdoptMe.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AdoptMeDbContext : IdentityDbContext
    {
        public AdoptMeDbContext(DbContextOptions<AdoptMeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; init; }

        public DbSet<Species> Species { get; init; }

        public DbSet<Shelter> Shelters { get; init; }

        public DbSet<City> Cities { get; init; }

        public DbSet<Address> Addresses { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Pet>()
                .HasOne(s => s.Species)
                .WithMany(s => s.Pets)
                .HasForeignKey(s => s.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Pet>()
                .HasOne(s => s.Shelter)
                .WithMany(s => s.Pets)
                .HasForeignKey(s => s.ShelterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Shelter>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Shelter>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Address>()
                .HasOne(a => a.City)
                .WithMany(a => a.Addresses)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Shelter>()
                .HasOne<Address>()
                .WithOne()
                .HasForeignKey<Shelter>(a => a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
