using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Caching;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Data.Repositories;
using LibraryManagement.Infrastructure.Identity;
using LibraryManagement.Infrastructure.Messaging.ServiceBus;
using LibraryManagement.Infrastructure.Storage;
using LibraryManagement.Infrastructure.Telemetry;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
       
        //Database
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    // Ensures migrations are placed in Infrastructure
                    npgsqlOptions.MigrationsAssembly(
                        typeof(LibraryDbContext).Assembly.FullName);
                }

                ));

        // Redis Cache
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "LibraryManagement:";
            });
        }
        else
        {
            // Fallback to in-memory cache for development
            services.AddDistributedMemoryCache();
        }

        // Services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddSingleton<IMessagePublisher, ServiceBusPublisher>();
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<ITelemetryService, AppInsightsTelemetryService>();

        // Identity Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        //Repositories
        services.AddScoped<IBookRepository, PostGresBookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();





        return services;
    }
}
