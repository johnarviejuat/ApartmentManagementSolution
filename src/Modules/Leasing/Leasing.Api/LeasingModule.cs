using AutoMapper;
using Billing.Application.Payments.Commands.Create;
using BuildingBlocks;
using Leasing.Application.Leases;
using Leasing.Application.Leases.Mapping;
using Leasing.Domain.Abstraction;
using Leasing.Infrastructure.Persistence;
using Leasing.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Leasing.Api;

public sealed class LeasingModule : IModule
{
    public string Name => "Leasing";
    public string? RoutePrefix => "leasing";
    public Assembly PresentationAssembly => typeof(LeasingApiAssemblyMarker).Assembly;

    public void Register(IServiceCollection services, IConfiguration cfg)
    {
        var cs =
            cfg.GetConnectionString(Name)
            ?? cfg.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Leasing connection string not configured.");

        services.AddDbContext<LeasingDbContext>(opt =>
            opt.UseSqlServer(
                cs,
                sql => sql
                    .MigrationsHistoryTable("__EFMigrationsHistory", "leasing")
                    .MigrationsAssembly(typeof(LeasingDbContext).Assembly.FullName)));
        services.AddScoped<ILeaseRepository, LeaseRepository>();
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(GetLeaseAgreementHandler).Assembly));
        services.AddAutoMapper(typeof(LeaseMappingProfile).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) { }
}
