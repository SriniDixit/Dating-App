
using DatingDAL;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContextPool<DataContext>(
            _options=>_options.UseSqlite(builder.Configuration.GetConnectionString("DatingSqllite"),
            _builder=>_builder.MigrationsAssembly("DatingAPI")
            )
        );
        builder.Services.AddCors();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(_policy=>_policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:4200"));
        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
