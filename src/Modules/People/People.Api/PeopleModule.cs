using AutoMapper;
using BuildingBlocks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using People.Api;
using People.Application.Owners.Commands.Create;
using People.Application.Owners.Mapping;
using People.Domain.Abstraction;
using People.Infrastructure.Persistence;
using People.Infrastructure.Persistence.Repositories;
using System.Reflection;

namespace People;

public sealed class PeopleModule : IModule
{
    public string Name => "People";
    public string? RoutePrefix => "people";
    public Assembly PresentationAssembly => typeof(PeopleApiAssemblyMarker).Assembly;

    public void Register(IServiceCollection services, IConfiguration cfg)
    {
        var cs =
            cfg.GetConnectionString(Name)
            ?? cfg.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("People connection string not configured.");

        services.AddDbContext<PeopleDbContext>(opt =>
            opt.UseSqlServer(
                cs,
                sql => sql
                    .MigrationsHistoryTable("__EFMigrationsHistory", "people")
                    .MigrationsAssembly(typeof(PeopleDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOwnerCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateOwnerCommand).Assembly,includeInternalTypes: true);
        services.AddAutoMapper(typeof(OwnerMappingProfile).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) { }
}
