using ApartmentManagement.Domain.Leasing;
using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Infrastructure;
using ApartmentManagement.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DDD wiring
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateApartmentHandler).Assembly));

// FluentValidation
builder.Services.AddScoped<IValidator<CreateApartmentCommand>, CreateApartmentValidator>();

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

