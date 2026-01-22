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
            // 1. Validation runs first
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // 2.Caching runs after validation
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
