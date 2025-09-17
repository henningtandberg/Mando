using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mando;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// This registers all implementations ICommandHandler&lt;TCommand&gt; as scoped, and IDispatcher as scoped
    /// </summary>
    /// <param name="services">The collection of services</param>
    /// <param name="assembly">The executing assembly</param>
    /// <returns>The collection of services with command handlers and the dispatcher added</returns>
    public static IServiceCollection AddMando(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.GetInterfaces().Any(i =>
                (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) ||
                (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));

        foreach (var t in handlerTypes)
        {
            foreach (var service in t.GetInterfaces()
                         .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))))
            {
                services.AddScoped(service, t);
            } 
            
            services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(ICommandHandler), t));
        }

        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }
}