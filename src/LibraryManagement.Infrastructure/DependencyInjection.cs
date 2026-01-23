using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Caching;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Data.Repositories;
using LibraryManagement.Infrastructure.Messaging.ServiceBus;
using LibraryManagement.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        //Repositories
        services.AddScoped<IBookRepository, PostGresBookRepository>();
        
       

       

        return services;
    }
}
