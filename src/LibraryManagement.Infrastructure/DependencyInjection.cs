using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Caching;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Data.Repositories;
using LibraryManagement.Infrastructure.Messaging.ServiceBus;
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

        //Repositories
        services.AddScoped<IBookRepository, PostGresBookRepository>();
        
        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        //// Caching (when I add Redis later)
        //// services.AddStackExchangeRedisCache(options =>
        ////     options.Configuration = configuration.GetConnectionString("Redis"));
        //// services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
