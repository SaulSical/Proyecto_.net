using AuthServiceIN6BV.Application.InTerface;
using AuthServiceIN6BV.Application.Services;
using AuthServiceIN6BV.Domain.Interface;
using AuthServiceIN6BV.Persistence.Data;
using AuthServiceIN6BV.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthServiceIN6BV.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(Options =>
        Options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddHealthChecks();

        return services;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    

    
}