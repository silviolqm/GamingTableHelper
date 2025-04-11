using NotificationService.SyncDataServices;

namespace NotificationService.Data
{
    public static class SeedApplicationUserData
    {
        public static void SeedUsers(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IApplicationUserDataClient>();
                var repo = serviceScope.ServiceProvider.GetService<INotificationRepo>();
                
                if (grpcClient == null || repo == null)
                {
                    Console.WriteLine($"Failed to seed data using gRPC.");
                }
                else
                {
                    var users = grpcClient.ReturnAllUsers();
                    foreach (var user in users)
                    {
                        if (!repo.ExternalUserExists(user.ExternalId))
                        {
                            repo.CreateUser(user);
                        }
                    }
                    repo.SaveChanges();
                }
            }
        }
    }
}