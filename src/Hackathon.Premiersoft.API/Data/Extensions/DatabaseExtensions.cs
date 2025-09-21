using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabseAsync(this WebApplication app)
        {
            var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PremiersoftHackathonDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
