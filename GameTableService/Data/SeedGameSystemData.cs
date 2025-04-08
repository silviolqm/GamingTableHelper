using GameTableService.SyncDataServices;

namespace GameTableService.Data
{
    public static class SeedGameSystemData
    {
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