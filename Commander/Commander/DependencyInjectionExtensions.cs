using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Commander;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCommander(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(i => new { Handler = t, Service = i }));

        foreach (var t in handlerTypes)
        {
            services.AddScoped(t.Service, t.Handler); // Register ICommandHandler<T>
            services.AddScoped(typeof(ICommandHandler), t.Handler); // Register also as non-generic
        }

        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }
}