using Billing.Application.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class ModuleExtensions
{
    private static IReadOnlyList<IModule> DiscoverModules()
    {
        var modules = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => {
                try { return a.GetTypes(); } catch { return []; }
            })
            .Where(t => typeof(IModule).IsAssignableFrom(t) && t is { IsAbstract: false, IsClass: true })
            .Select(t => (IModule)Activator.CreateInstance(t)!)
            .OrderBy(m => m.Name)
            .ToList();
        return modules;
    }
    public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration configuration)
    {
        var mvc = services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new TenantRefIdJsonConverter());
                o.JsonSerializerOptions.Converters.Add(new ApartmentRefIdJsonConverter());
            });
        var modules = DiscoverModules();
        foreach (var m in modules)
        {
            if (m.PresentationAssembly is not null)
            {
                mvc.PartManager.ApplicationParts.Add(
                    new AssemblyPart(m.PresentationAssembly));
            }
            m.Register(services, configuration);
        }
        services.AddSingleton<IReadOnlyList<IModule>>(modules);
        return services;
    }
    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var modules = endpoints.ServiceProvider.GetRequiredService<IReadOnlyList<IModule>>();

        foreach (var m in modules)
        {
            var group = !string.IsNullOrWhiteSpace(m.RoutePrefix)
                ? endpoints.MapGroup($"/{m.RoutePrefix}")
                : endpoints;

            m.MapEndpoints(group);
        }
        endpoints.MapControllers();
        return endpoints;
    }
}
