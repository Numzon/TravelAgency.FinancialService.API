using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.API.Infrastructure.Persistance;

namespace TravelAgency.FinancialService.API.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection RegisterDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IFinancialServiceDbContext, FinancialServiceDbContext>();

        return services;
    }
}
