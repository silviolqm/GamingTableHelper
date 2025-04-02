using GameSystemService.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSystemService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<GameSystem> GameSystems { get; set; }
    }
}