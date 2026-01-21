using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using System.Runtime.CompilerServices;
using FluentAssertions.Common;


namespace LibraryManagement.Application;

public static class DependencyInjection 
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR - handles commands/queries
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation - auto-register validators
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper 
        services.AddAutoMapper(assembly);

        return services;
    }

}
