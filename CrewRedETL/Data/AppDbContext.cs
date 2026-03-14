using Microsoft.EntityFrameworkCore;
using CrewRedETL.Models;

namespace CrewRedETL.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TripDataEntity> TripData { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TripDataEntity>(entity =>
            {
                entity.ToTable("TripData");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PickupDatetime).HasColumnType("datetime2");
                entity.Property(e => e.DropoffDatetime).HasColumnType("datetime2");
                entity.Property(e => e.PassengerCount).HasColumnType("tinyint");
                entity.Property(e => e.TripDistance).HasColumnType("float");
                entity.Property(e => e.StoreAndFwdFlag).HasColumnType("nvarchar(3)");
                entity.Property(e => e.PULocationId).HasColumnType("smallint");
                entity.Property(e => e.DOLocationId).HasColumnType("smallint");
                entity.Property(e => e.FareAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TipAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TravelTimeSeconds)
                    .HasColumnName("travel_time_seconds")
                    .HasComputedColumnSql(
                        "CASE " +
                        "WHEN [PickupDatetime] IS NULL OR [DropoffDatetime] IS NULL THEN NULL " +
                        "ELSE DATEDIFF(SECOND, [PickupDatetime], [DropoffDatetime]) " +
                        "END", false);

                // 4.1 + 4.4
                entity.HasIndex(e => e.PULocationId).HasDatabaseName("IX_PULocationId_TipAmount");
                // 4.2
                entity.HasIndex(e => e.TripDistance).HasDatabaseName("IX_TripDistance");
                // 4.3
                entity.HasIndex(e => new { e.PickupDatetime, e.DropoffDatetime }).HasDatabaseName("IX_Duration");
            });
        }
    }
}