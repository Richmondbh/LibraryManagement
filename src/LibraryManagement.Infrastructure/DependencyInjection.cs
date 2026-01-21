using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        //// Database
        //services.AddDbContext<LibraryDbContext>(options =>
        //    options.UseSqlServer(
        //        configuration.GetConnectionString("DefaultConnection")));

        //// Repositories
        //services.AddScoped<IBookRepository, SqlBookRepository>();
        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        //// Caching (when you add Redis later)
        //// services.AddStackExchangeRedisCache(options =>
        ////     options.Configuration = configuration.GetConnectionString("Redis"));
        //// services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
