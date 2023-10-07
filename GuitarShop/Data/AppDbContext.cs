using GuitarShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Data
{
    public class AppDbContext :IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<FacilityManager> FacilityManagers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Facility> Facilities { get; set; } 
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<FacilityInCharge> FacilitiesInCharge { get; set; }
        public DbSet<User> UserS  { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<FacilityManager>().ToTable("FacilityManager");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Facility>().ToTable("Facility");
            modelBuilder.Entity<Facility>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Facilities)
                .HasForeignKey(f => f.CategoryID);
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<FacilityInCharge>().ToTable("FacilityInCharge");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

         
        }

    }
}
