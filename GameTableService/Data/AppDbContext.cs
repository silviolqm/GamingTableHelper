using GameTableService.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTableService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<GameSystem> GameSystems { get; set; }
        public DbSet<GameTable> GameTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameTable>()
            .HasOne(gt => gt.GameSystem)
            .WithMany(gs => gs.GameTables)
            .HasForeignKey(gt => gt.GameSystemId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}