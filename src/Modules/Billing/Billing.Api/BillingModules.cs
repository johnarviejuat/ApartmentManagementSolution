using Billing.Application;
using Billing.Application.Payments.Commands.Create;
using Billing.Application.Payments.Mapping;
using Billing.Domain.Abstraction;
using Billing.Infrastructure.Persistence;
using Billing.Infrastructure.Persistence.Repositories;
using BuildingBlocks;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Billing.Api;

public sealed class BillingModule : IModule
{
    public string Name => "Billing";
    public string? RoutePrefix => "billing";
    public Assembly PresentationAssembly => typeof(BillingApiAssemblyMarker).Assembly;

    public void Register(IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString(Name)
           ?? cfg.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Catalog connection string not configured.");

        services.AddDbContext<BillingDbContext>(opt =>
            opt.UseSqlServer(
                cs,
                sql => sql.MigrationsAssembly(typeof(BillingDbContext).Assembly.FullName)
            ));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreatePaymentCommand).Assembly, includeInternalTypes: true);
        services.AddAutoMapper(typeof(PaymentMappingProfile));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints){}
}
