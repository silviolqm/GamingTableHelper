using GameTableService.SyncDataServices;
using Microsoft.EntityFrameworkCore;

namespace GameTableService.Data
{
    public static class PrepDatabase
    {
        public static void DoMigrations(IApplicationBuilder app, bool isProduction) 
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<AppDbContext>();
            try
            {
                if (isProduction && context != null)
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not run migrations: {ex.Message}");
            }
        }
        
        public static void SeedGameSystems(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IGameSystemDataClient>();
                var repo = serviceScope.ServiceProvider.GetService<IGameTableRepo>();
                
                if (grpcClient == null || repo == null)
                {
                    Console.WriteLine($"Failed to seed data using gRPC.");
                }
                else
                {
                    var gameSystems = grpcClient.ReturnAllGameSystems();
                    if (gameSystems == null)
                    {
                        Console.WriteLine($"No GameSystems found in gRPC connection to GameSystemService.");
                        return;
                    }
                    foreach (var gameSystem in gameSystems)
                    {
                        if (!repo.ExternalGameSystemExists(gameSystem.ExternalId))
                        {
                            repo.CreateGameSystem(gameSystem);
                        }
                    }
                    repo.SaveChanges();
                }
            }
        }
    }
}