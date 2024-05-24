using TravelAgency.FinancialService.API.Common.Models;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface ITaxRepository
{
    Task<IEnumerable<TaxDto>> ListWithTaxTypeAsync(DateTime TransactionDate, CancellationToken cancellationToken);
}
