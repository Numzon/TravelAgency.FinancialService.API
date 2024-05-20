using TravelAgency.FinancialService.API.Domain.Entities;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface ITaxRepository
{
    Task<IEnumerable<Tax>> ListAsync(DateTime TransactionDate, CancellationToken cancellationToken);
}
