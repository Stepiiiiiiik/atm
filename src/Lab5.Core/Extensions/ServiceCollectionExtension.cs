using Lab5.Core.Abstractions;
using Lab5.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lab5.Core.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<IPinHashService, PinHashService>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<ISessionService, SessionService>();

        return services;
    }
}