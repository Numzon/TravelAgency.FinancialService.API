using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.Tests.Shared.Interfaces;
public interface IPrepopulateStrategy<TDbContext>
    where TDbContext : IFinancialServiceDbContext
{
    Task PrepopulateAsync(TDbContext context, IFixture fixture);
}
