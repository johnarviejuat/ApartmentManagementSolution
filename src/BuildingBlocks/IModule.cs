using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public interface IModule
{
    string Name { get; }
    string? RoutePrefix => null;
    Assembly PresentationAssembly => GetType().Assembly;
    void Register(IServiceCollection services, IConfiguration configuration);
    void MapEndpoints(IEndpointRouteBuilder endpoints) { }
}
