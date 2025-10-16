using JuiceLlama.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JuiceLlama.Common.Extensions
{
    public static class AddTypesFromAssembly
    {
        /// <summary>
        /// Registers types from the specified assembly that implement interfaces derived from <typeparamref name="TBase"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method scans the provided assembly for concrete classes that implement interfaces inheriting from <typeparamref name="TBase"/>.
        /// Each matching class is registered as a scoped service with its most specific derived interface (excluding <typeparamref name="TBase"/> itself).
        /// </para>
        /// <para>
        /// Classes that implement <see cref="IDecorator"/> are excluded from registration, allowing separation between
        /// core implementations and decorator patterns.
        /// </para>
        /// <para>
        /// Example: If <typeparamref name="TBase"/> is <c>IRepository</c>, and a class implements <c>IUserRepository</c>
        /// (which inherits from <c>IRepository</c>), the class will be registered as <c>IUserRepository</c> with scoped lifetime.
        /// </para>
        /// </remarks>
        /// <typeparam name="TBase">The base interface type used to filter implementable interfaces.</typeparam>
        /// <param name="services">The service collection to register the types with.</param>
        /// <param name="assembly">The assembly to scan for types.</param>
        /// <returns>The service collection with registered types for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TBase"/> is not an interface.</exception>
        public static IServiceCollection AddAssemblyTypes<TBase>(this IServiceCollection services, Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            if (!typeof(TBase).IsInterface)
                throw new ArgumentException("TBase must be an interface type", typeof(TBase).ToString());

            var types = assembly.GetTypes()
                .Where(t => !typeof(IDecorator).IsAssignableFrom(t)
                            && typeof(TBase).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract);

            foreach (var type in types)
            {
                // Get all interfaces implemented by the class and find the first one
                // that derives from TBase but isn't TBase itself
                var implementedInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(TBase) &&
                                        typeof(TBase).IsAssignableFrom(i));

                if (implementedInterface != null)
                    services.AddScoped(implementedInterface, type);
            }
            return services;
        }

        /// <summary>
        /// Registers decorators for all implementations of the specified interface in the assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// 1. Finds all concrete (non-abstract) classes in the specified assembly that implement the <typeparamref name="TDecorated"/> interface.
        /// 2. Filters classes that implement the <typeparamref name="TDecorator"/> interface (decorator marker).
        /// 3. For each matching class, finds the first interface that:
        ///    - Is not <typeparamref name="TDecorator"/> or <typeparamref name="TDecorated"/>
        ///    - Is derived from <typeparamref name="TDecorated"/>
        /// 4. Registers the decorator for the found interface using the decorator type.
        /// </para>
        /// <para>
        /// Example: if TDecorated = IRepository and TDecorator = ILoggingDecorator,
        /// the method will find classes implementing both IRepository and ILoggingDecorator, and register them
        /// as decorators for the corresponding repository interface (e.g., IUserRepository).
        /// </para>
        /// </remarks>
        /// <typeparam name="TDecorator">Marker interface that decorator classes must implement.</typeparam>
        /// <typeparam name="TDecorated">Base interface for which decorators are applied.</typeparam>
        /// <param name="services">Service collection for registration.</param>
        /// <param name="assembly">Assembly to search for classes.</param>
        /// <returns>Service collection with registered decorators.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentException">When <typeparamref name="TDecorator"/> or <typeparamref name="TDecorated"/> are not interfaces.</exception>
        public static IServiceCollection AddAssemblyDecoratorTypes<TDecorator, TDecorated>(this IServiceCollection services, Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            if (!typeof(TDecorator).IsInterface)
                throw new ArgumentException("TBase must be an interface type", typeof(TDecorator).ToString());

            if (!typeof(TDecorated).IsInterface)
                throw new ArgumentException("TBase must be an interface type", typeof(TDecorated).ToString());

            var types = assembly.GetTypes()
                .Where(t => typeof(TDecorated).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract);

            foreach (var type in types)
            {
                if (!type.GetInterfaces().Contains(typeof(TDecorator)))
                    continue;

                // Find the target interface for decoration (derived from TDecorated, but not TDecorated/TDecorator themselves)
                var implementedInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(TDecorator)
                                        && i != typeof(TDecorated)
                                        && typeof(TDecorated).IsAssignableFrom(i));

                if (implementedInterface != null)
                    services.Decorate(implementedInterface, type);
            }
            return services;
        }
    }
}