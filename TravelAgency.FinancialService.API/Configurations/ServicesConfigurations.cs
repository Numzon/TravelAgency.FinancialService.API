using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.API.Infrastructure.Repositories;
using TravelAgency.FinancialService.API.Infrastructure.Services;

namespace TravelAgency.FinancialService.API.Configurations;

public static class ServicesConfigurations
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
