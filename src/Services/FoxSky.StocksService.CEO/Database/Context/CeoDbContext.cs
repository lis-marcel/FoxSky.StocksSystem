using FoxSky.StocksService.Ceo.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxSky.StocksService.Ceo.Database.Context
{
    public class CeoDbContext : DbContext
    {
        public DbSet<Report> Reports { get; set; }

        public CeoDbContext(DbContextOptions<CeoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.ReportId).IsRequired();
                entity.Property(e => e.DmsDocumentId).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
