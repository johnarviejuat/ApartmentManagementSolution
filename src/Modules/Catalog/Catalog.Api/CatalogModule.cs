using System.Reflection;
using AutoMapper;
using BuildingBlocks;
using Catalog.Application.Apartments.Commands.Create;
using Catalog.Application.Apartments.Mapping;
using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Api;

public sealed class CatalogModule : IModule
{
    public string Name => "Catalog";
    public string? RoutePrefix => "catalog";
    public Assembly PresentationAssembly => typeof(CatalogApiAssemblyMarker).Assembly;
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var cs =
            configuration.GetConnectionString(Name)
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Catalog connection string not configured.");

        services.AddDbContext<CatalogDbContext>(opt =>
            opt.UseSqlServer(
                cs,
                sql => sql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateApartmentCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateApartmentCommand).Assembly,includeInternalTypes: true);
        services.AddAutoMapper(typeof(ApartmentMappingProfile).Assembly);
    }
    public void MapEndpoints(IEndpointRouteBuilder endpoints) { }
}
