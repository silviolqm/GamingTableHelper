using Microsoft.EntityFrameworkCore;
using NotificationService.SyncDataServices;

namespace NotificationService.Data
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
                    if (users == null)
                    {
                        Console.WriteLine($"No users found in gRPC connection to AuthService.");
                        return;
                    }
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