using Lab5.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.DataAccess.Postgres.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IAccountStorage, AccountStorage>();
        services.AddSingleton<IHistoryStorage, HistoryStorage>();
        services.AddSingleton<ISessionStorage, SessionStorage>();
        services.AddSingleton<IOperationTypeStorage, OperationTypeStorage>();
        services.AddSingleton<ISessionTypeStorage, SessionTypeStorage>();
        services.AddSingleton<DapperContext>();
        services.Configure<DatabaseSettings>(x => x.ConnectionString = connectionString);

        return services;
    }
}