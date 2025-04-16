using Microsoft.EntityFrameworkCore;

namespace GameSystemService.Data
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
    }
}