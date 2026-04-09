using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChitMeo.Mediator;

/// <summary>
/// Provides extension methods for configuring mediator services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the mediator services to the dependency injection container with default options.
    /// </summary>
    /// <param name="services">The service collection to add mediator services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services)
        => services.AddMediator(new MediatorOptions());

    /// <summary>
    /// Adds the mediator services to the dependency injection container with custom options.
    /// </summary>
    /// <param name="services">The service collection to add mediator services to.</param>
    /// <param name="configure">A delegate to configure <see cref="MediatorOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Action<MediatorOptions> configure)
    {
        var options = new MediatorOptions();
        configure(options);
        return services.AddMediator(options);
    }

    private static IServiceCollection AddMediator(
        this IServiceCollection services,
        MediatorOptions options)
    {
        services.TryAddScoped<IMediator, Mediator>();

        var modulePrefix = options.ModulePrefix;

        // Load assemblies from disk (normal publish)
        var files = Directory.GetFiles(AppContext.BaseDirectory, $"*{modulePrefix}*.dll");

        foreach (var file in files)
        {
            try
            {
                Assembly.Load(AssemblyName.GetAssemblyName(file));
            }
            catch
            {
            }
        }

        // Scan loaded assemblies (works with SingleFile)
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => a.GetName().Name!.Contains(modulePrefix))
            .ToArray();

        foreach (var assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            var handlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .Select(i => new
                    {
                        Service = i,
                        Implementation = t
                    }));

            foreach (var handler in handlers)
            {
                services.TryAddScoped(handler.Service, handler.Implementation);
            }
        }

        return services;
    }
}