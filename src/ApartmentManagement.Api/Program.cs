
using Billing.Api;
using BuildingBlocks;
using Catalog.Api;
using Catalog.Domain.Entities;
using Leasing.Api;
using Microsoft.OpenApi.Models;
using People;
using People.Domain.Entities;
using Scalar.AspNetCore;

_ = typeof(BillingModule).Assembly;
_ = typeof(CatalogModule).Assembly;
_ = typeof(LeasingModule).Assembly;
_ = typeof(PeopleModule).Assembly;


var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterModules(builder.Configuration);
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
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapModuleEndpoints();

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

app.Run();
