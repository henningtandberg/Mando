using System.Reflection;
using Mando.Command;
using Mando.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mando;

public static class DependencyInjectionExtensions
{
    /// <param name="services">The collection of services</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// This registers all implementations of Mando features, if any
        /// </summary>
        /// <param name="assembly">The executing assembly</param>
        /// <returns>Service Collection with implementations of Mando features added</returns>
        public IServiceCollection AddMando(Assembly assembly)
        {
            return services
                .AddMediator(assembly)
                .AddEventBus(assembly);
        }

        /// <summary>
        /// Registers a pipeline behavior. Behaviors run around command handlers in the order they
        /// are registered (first registered is the outermost layer). Accepts closed types
        /// (e.g. typeof(ValidateOrderBehavior)) and open generic types (e.g. typeof(LoggingBehavior&lt;&gt;))
        /// which apply to every command.
        /// </summary>
        /// <param name="behaviorType">The behavior implementation type</param>
        /// <returns>The collection of services with the behavior added</returns>
        public IServiceCollection AddPipelineBehavior(Type behaviorType)
        {
            var behaviorInterfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<>) ||
                             i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
                .ToList();

            if (behaviorInterfaces.Count == 0)
                throw new ArgumentException(
                    $"{behaviorType} does not implement IPipelineBehavior<> or IPipelineBehavior<,>", nameof(behaviorType));

            foreach (var behaviorInterface in behaviorInterfaces)
            {
                var serviceType = behaviorType.IsGenericTypeDefinition
                    ? behaviorInterface.GetGenericTypeDefinition()
                    : behaviorInterface;

                services.AddTransient(serviceType, behaviorType);
            }

            return services;
        }

        /// <summary>
        /// Registers a pipeline behavior. See <see cref="AddPipelineBehavior(Type)"/>.
        /// </summary>
        /// <typeparam name="TBehavior">The behavior implementation type</typeparam>
        /// <returns>The collection of services with the behavior added</returns>
        public IServiceCollection AddPipelineBehavior<TBehavior>() where TBehavior : class
            => services.AddPipelineBehavior(typeof(TBehavior));

        /// <summary>
        /// This registers all implementations ICommandHandler&lt;TCommand&gt; as scoped, and IDispatcher as scoped
        /// </summary>
        /// <param name="assembly">The executing assembly</param>
        /// <returns>The collection of services with command handlers and the dispatcher added</returns>
        private IServiceCollection AddMediator(Assembly assembly)
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
        /// <param name="assembly">The executing assembly</param>
        /// <returns>The collection of services with event subscribers and the event bus added</returns>
        private IServiceCollection AddEventBus(Assembly assembly)
        {
            var subscriberImplementations = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)));

            foreach (var subscriberImplementation in subscriberImplementations)
            {
                services.TryAddSingleton(subscriberImplementation);

                foreach (var subscriberInterface in subscriberImplementation.GetInterfaces()
                             .Where(i => i.IsGenericType &&
                                         i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>)))
                {
                    services.AddSingleton(subscriberInterface, sp =>
                        sp.GetRequiredService(subscriberImplementation));
                }
            }

            services.AddSingleton<IEventBus, EventBus>();

            return services;
        }
    }
}