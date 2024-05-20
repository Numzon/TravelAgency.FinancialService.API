using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.API.Infrastructure.Repositories;

namespace TravelAgency.FinancialService.API.Configurations;

public static class RepositoriesConfiguration
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITaxTypeRepository, TaxTypesRepository>();
        services.AddScoped<IServiceFeeRepository, ServiceFeeRepository>();
        services.AddScoped<IFinancialReportRepository, FinancialReportRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<ITaxFinancialReportRepository, TaxFinancialReportRepository>();

        return services;
    }
}
