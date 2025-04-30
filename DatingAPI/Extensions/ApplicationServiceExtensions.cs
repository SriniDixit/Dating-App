using DatingDAL;
using DatingServices;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContextPool<DataContext>(
            options => options.UseSqlite(configuration.GetConnectionString("DatingSqllite"),
            builder => builder.MigrationsAssembly("DatingAPI")
            )
        );
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddCors();

        return services;
    }
}