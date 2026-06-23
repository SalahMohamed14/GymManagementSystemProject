using GymManagementSystem.DAL.Data.DataSeed;
using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystemProject;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.PL
{
    public static class ProgramExtensions
    {
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var pending = await dbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                logger.LogInformation($"Applying {pending.Count()} Pending Migrations");
            }

            var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await DataSeeding.SeedAsync(dbContext, seedPath, logger);
        }
    }
}
