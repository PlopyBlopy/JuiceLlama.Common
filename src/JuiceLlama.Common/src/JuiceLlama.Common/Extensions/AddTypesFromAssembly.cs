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
                throw new ArgumentException("TBase must be an interface type", nameof(TBase));

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
        /// Registers types from the specified assembly that implement the specified generic interface definition.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method scans the provided assembly for concrete classes that implement closed versions of the specified open generic interface.
        /// Each matching class is registered as a scoped service with its specific closed generic interface.
        /// </para>
        /// <para>
        /// Classes that implement <see cref="IDecorator"/> are excluded from registration, ensuring separation between
        /// core implementations and decorator patterns.
        /// </para>
        /// <para>
        /// Example: If <paramref name="genericInterfaceType"/> is <c>IRepository&lt;TRequest, TResponse&gt;</c>, and a class implements
        /// <c>IRepository&lt;UserRequest, UserResponse&gt;</c>, the class will be registered as <c>IRepository&lt;UserRequest, UserResponse&gt;</c>
        /// with scoped lifetime.
        /// </para>
        /// <para>
        /// The method only supports generic interfaces with exactly two type parameters to ensure type safety and proper dependency resolution.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to register the types with.</param>
        /// <param name="assembly">The assembly to scan for types.</param>
        /// <param name="genericInterfaceType">The open generic interface type to scan for implementations (e.g., IRepository&lt;,&gt;).
        /// Must be a generic type definition with exactly two type parameters.</param>
        /// <returns>The service collection with registered types for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="genericInterfaceType"/> is not a generic type definition
        /// or does not have exactly two type parameters.</exception>
        public static IServiceCollection AddAssemblyTypes(this IServiceCollection services, Assembly assembly, Type genericInterfaceType)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            if (!genericInterfaceType.IsGenericTypeDefinition || genericInterfaceType.GetGenericArguments().Length != 2)
                throw new ArgumentException("Type must be a generic interface with two type parameters.", nameof(genericInterfaceType));

            var types = assembly.GetTypes()
                        .Where(t => t.IsClass
                                    && !t.IsAbstract
                                    && !typeof(IDecorator).IsAssignableFrom(t))
                        .Where(t => t.GetInterfaces()
                                    .Any(i => i.IsGenericType
                                              && i.GetGenericTypeDefinition() == genericInterfaceType));

            foreach (var type in types)
            {
                var implementedInterface = type.GetInterfaces()
                                                .FirstOrDefault(i => i.IsGenericType
                                                                     && i.GetGenericTypeDefinition() == genericInterfaceType);

                if (implementedInterface is not null)
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
                throw new ArgumentException("TBase must be an interface type", nameof(TDecorator));

            if (!typeof(TDecorated).IsInterface)
                throw new ArgumentException("TBase must be an interface type", nameof(TDecorated));

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

        /// <summary>
        /// Registers decorator types from the specified assembly that implement the specified generic interface.
        /// This method supports both open-generic decorators (e.g., LoggerDecorator&lt;T1, T2&gt;)
        /// and closed-generic decorators (e.g., LoggerDecorator&lt;ConcreteType1, ConcreteType2&gt;).
        /// </summary>
        /// <typeparam name="TDecorator">The base decorator type or interface that decorator classes must implement.
        /// This is typically a marker interface like ILoggerDecorator or a base decorator class.</typeparam>
        /// <param name="services">The service collection to register the decorators with.</param>
        /// <param name="assembly">The assembly to scan for decorator types.</param>
        /// <param name="genericInterfaceDecoratedType">The generic interface definition (open generic) that the decorators wrap.
        /// Must be a generic interface with exactly two type parameters (e.g., IRepository&lt;TRequest, TResponse&gt;).</param>
        /// <returns>The service collection for method chaining.</returns>
        /// <exception cref="ArgumentException">Thrown when the genericInterfaceDecoratedType is not a generic type definition
        /// or does not have exactly two type parameters.</exception>
        /// <remarks>
        /// For open-generic decorators (e.g., LoggerDecorator&lt;,&gt;), the method registers them directly with the open generic interface.
        /// For closed-generic decorators (e.g., SpecificLoggerDecorator), the method finds the specific implemented interface and registers accordingly.
        /// </remarks>
        public static IServiceCollection AddAssemblyDecoratorTypes<TDecorator>(this IServiceCollection services, Assembly assembly, Type genericInterfaceDecoratedType)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            // Validate that the provided type is a generic interface definition with exactly two type parameters
            if (!genericInterfaceDecoratedType.IsGenericTypeDefinition || genericInterfaceDecoratedType.GetGenericArguments().Length != 2)
                throw new ArgumentException("Type must be a generic interface with two type parameters.", nameof(genericInterfaceDecoratedType));

            // Find all non-abstract classes in the assembly that implement TDecorator and the specified generic interface
            var types = assembly.GetTypes()
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && typeof(TDecorator).IsAssignableFrom(t))
                .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType
                                        && i.GetGenericTypeDefinition() == genericInterfaceDecoratedType));

            foreach (var type in types)
            {
                if (type.IsGenericTypeDefinition)
                {
                    // Register open-generic decorator (e.g., LoggerDecorator<,>) with the open generic interface
                    services.Decorate(genericInterfaceDecoratedType, type);
                }
                else
                {
                    // For closed-generic decorators, find the specific implemented interface
                    // Exclude the TDecorator interface itself from the search
                    var implementedInterface = type.GetInterfaces()
                        .FirstOrDefault(i => i != typeof(TDecorator)
                                            && i.IsGenericType
                                            && i.GetGenericTypeDefinition() == genericInterfaceDecoratedType);

                    // Register the decorator for the specific closed-generic interface
                    if (implementedInterface is not null)
                        services.Decorate(implementedInterface, type);
                }
            }

            return services;
        }
    }
}