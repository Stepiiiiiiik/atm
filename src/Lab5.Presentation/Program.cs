using Lab5.Core.Extensions;
using Lab5.DataAccess.Postgres.Extensions;
using Lab5.DataAccess.Postgres.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Configuration, builder.Services);

        WebApplication app = builder.Build();
        Configure(app);

        DatabaseExtension.MigrateDatabase(builder.Configuration.GetConnectionString("postgres") ?? string.Empty);

        app.Run();
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
        services.AddCore();
        services.AddDataAccess(configuration.GetConnectionString("Postgres") ?? string.Empty);
    }

    private static void Configure(WebApplication app)
    {
        app.UseSwagger(); // добавлет эндпоинт swagger/v1/swagger.json где будет описана спецификация нашего API
        app.UseSwaggerUI(); // Добавляет UI по эндпоинту swagger/index.html, который генерируется на основе swagger.json
        app.MapControllers(); // вызывает нужный контроллер в зависимости от запрашиваего урла
    }
}