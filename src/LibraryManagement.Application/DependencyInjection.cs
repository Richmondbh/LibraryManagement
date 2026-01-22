using FluentAssertions.Common;
using FluentValidation;
using LibraryManagement.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


namespace LibraryManagement.Application;

public static class DependencyInjection 
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR - handles commands/queries
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Register pipeline behaviors
            // 1. Logging runs first (captures everything)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // 2. Validation runs second
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // 3. Caching runs last (for queries)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        });


        // FluentValidation - auto-register validators
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper 
        //services.AddAutoMapper(assembly);

        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

}
