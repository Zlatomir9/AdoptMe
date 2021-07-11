namespace AdoptMe.Data
{
    using AdoptMe.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class AdoptMeDbContext : DbContext
    {
        public AdoptMeDbContext(DbContextOptions<AdoptMeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; init; }

        public DbSet<Species> Species { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Pet>()
                .HasOne(s => s.Species)
                .WithMany(s => s.Pets)
                .HasForeignKey(s => s.SpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
