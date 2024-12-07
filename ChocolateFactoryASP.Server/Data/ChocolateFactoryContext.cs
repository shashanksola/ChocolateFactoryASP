using Microsoft.EntityFrameworkCore;
using ChocolateFactory.Models;
namespace ChocolateFactory.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<ProductionSchedule> ProductionSchedules { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<QualityCheck> QualityChecks { get; set; }
        public DbSet<FinishedGood> FinishedGoods { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RawMaterial>()
                .Property(r => r.CostPerUnit)
                .HasColumnType("decimal(18,4)");
        }
    }
}