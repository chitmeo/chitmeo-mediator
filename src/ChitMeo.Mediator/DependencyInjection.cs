using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChitMeo.Mediator;

public static class DependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();

        const string modulePrefix = ".Module.";

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
