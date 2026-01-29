using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Constants;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<LibraryDbContext>>();

            try
            {
                // Applying any pending migrations
                await context.Database.MigrateAsync();

                // Seeding admin user if none exists
                if (!await context.Users.AnyAsync(u => u.Role == Roles.Admin))
                {
                    var adminEmail = "richmondboakye0017@icloud.com";
                    var adminPassword = "richmond";

                    var admin = User.Create(
                        adminEmail,
                        passwordHasher.Hash(adminPassword),
                        "System",
                        "Admin",
                        Roles.Admin
                    );

                    await context.Users.AddAsync(admin);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Seeded admin user: {Email}", adminEmail);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}
