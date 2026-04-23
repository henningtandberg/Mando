using System.Reflection;
using Mando.Command;
using Mando.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mando;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// This registers all implementations of Mando features, if any
    /// </summary>
    /// <param name="services">The collection of services</param>
    /// <param name="assembly">The executing assembly</param>
    /// <returns>Service Collection with implementations of Mando features added</returns>
    public static IServiceCollection AddMando(this IServiceCollection services, Assembly assembly)
    {
        return services
            .AddMediator(assembly)
            .AddEventBus(assembly);
    }
    
    /// <summary>
    /// This registers all implementations ICommandHandler&lt;TCommand&gt; as scoped, and IDispatcher as scoped
    /// </summary>
    /// <param name="services">The collection of services</param>
    /// <param name="assembly">The executing assembly</param>
    /// <returns>The collection of services with command handlers and the dispatcher added</returns>
    private static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
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

    /// <summary>
    /// Registers all IEventSubscriber&lt;TEvent&gt; implementations as singletons and registers IEventBus.
    /// Subscribers implementing multiple IEventSubscriber interfaces share the same instance.
    /// </summary>
    /// <param name="services">The collection of services</param>
    /// <param name="assembly">The executing assembly</param>
    /// <returns>The collection of services with event subscribers and the event bus added</returns>
    private static IServiceCollection AddEventBus(this IServiceCollection services, Assembly assembly)
    {
        var subscriberTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)));

        foreach (var concreteType in subscriberTypes)
        {
            services.TryAddSingleton(concreteType);

            foreach (var iface in concreteType.GetInterfaces()
                         .Where(i => i.IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)))
            {
                var capturedIface = iface;
                var capturedConcrete = concreteType;
                services.AddSingleton(capturedIface, sp => sp.GetRequiredService(capturedConcrete));
            }
        }

        services.AddSingleton<IEventBus, EventBus>();

        return services;
    }
}