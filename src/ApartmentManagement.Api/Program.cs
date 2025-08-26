using ApartmentManagement.Application.Apartments;

using ApartmentManagement.Application.Apartments.Mapping;
using ApartmentManagement.Application.Behaviors;
using ApartmentManagement.Application.Common;
using ApartmentManagement.Application.Owners.Mapping;
using ApartmentManagement.Application.Payments;
using ApartmentManagement.Application.Payments.Mapping;
using ApartmentManagement.Application.Tenants;
using ApartmentManagement.Application.Tenants.Mapping;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Leases;
using ApartmentManagement.Domain.Leasing.Owners;
using ApartmentManagement.Domain.Leasing.Payments;
using ApartmentManagement.Domain.Leasing.Tenants;
using ApartmentManagement.Infrastructure;
using ApartmentManagement.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new TenantIdJsonConverter());
        o.JsonSerializerOptions.Converters.Add(new ApartmentIdJsonConverter());
    });
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Components ??= new();
        doc.Components.SecuritySchemes["BearerAuth"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization"
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer((op, ctx, ct) =>
    {
        var hasAuth = ctx.Description.ActionDescriptor?.EndpointMetadata?
            .OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>()
            .Any() == true;

        if (hasAuth)
        {
            op.Security ??= new List<OpenApiSecurityRequirement>();
            op.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "BearerAuth" }
                }] = Array.Empty<string>()
            });
        }
        return Task.CompletedTask;
    });

    options.AddSchemaTransformer((schema, context, ct) =>
    {
        var t = context.JsonTypeInfo?.Type;
        var underlying = t is null ? null : Nullable.GetUnderlyingType(t) ?? t;

        if (underlying == typeof(TenantId) || underlying == typeof(ApartmentId))
        {
            schema.Type = "string";
            schema.Format = "uuid";
            schema.Properties?.Clear();
            schema.AdditionalPropertiesAllowed = false;
        }
        return Task.CompletedTask;
    });

});


// EF Core
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DDD wiring
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ILeaseRepository, LeaseRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateApartmentHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CreateOwnerHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CreateTenantHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CreatePaymentHandler>();
});

// FluentValidation
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies(),includeInternalTypes: true);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// AutoMapper
builder.Services.AddAutoMapper(typeof(ApartmentMappingProfile));
builder.Services.AddAutoMapper(typeof(OwnerMappingProfile));
builder.Services.AddAutoMapper(typeof(TenantMappingProfile));
builder.Services.AddAutoMapper(typeof(PaymentMappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(o =>
    {
        o.WithTitle("Apartment Management API");
        o.WithTheme(ScalarTheme.DeepSpace);
        o.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http);
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();




