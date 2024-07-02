using HolyShift;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddAuthentication("Bearer").AddScheme<AuthenticationSchemeOptions, AccessTokenHandler>("Bearer", null);
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContext<HolyShiftContext>(ServiceLifetime.Transient)
    .AddTransient<EmployeesLogic>()
    .AddTransient<AuthLogic>()
    .AddLogging(config =>
    {
        config.AddConsole();
        config.AddDebug();
        // Other logging providers
    });

ConfigureSwagger(builder.Services);

using (HolyShiftContext DB = new HolyShiftContext())
{
    DB.Database.Migrate();
    if (!DB.Employees.Any())
    {
        string password = "admin1234";
        byte[] hash = EncryptDecryptHelper.DerivePublicKey(password);
        DB.Employees.Add(new Employee
        {
            Email = "david1ariel@gmail.com",
            Name = "David Ariel",
            PasswordHash = hash,
            Role = "Manager",
            Username = "David"
        });
        DB.SaveChanges();
    }

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger()
            .UseSwaggerUI();
    }

    app.UseHttpsRedirection()
        .UseAuthentication()
        .UseAuthorization();

    app.MapControllers();

    app.Run();

}

static void ConfigureSwagger(IServiceCollection services)
{
    // Register the Swagger generator, defining 1 or more Swagger documents
    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
        {
            Name = "JWT Token",
            Description = "Authorization header with Bearer scheme using Elliptic Curve Cryptography",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });


        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
        });
    });
}