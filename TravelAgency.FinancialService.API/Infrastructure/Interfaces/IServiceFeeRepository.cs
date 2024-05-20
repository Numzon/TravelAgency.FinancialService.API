using TravelAgency.FinancialService.API.Domain.Entities;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface IServiceFeeRepository
{
    Task<IEnumerable<ServiceFee>> ListByTransactionDateAsync(DateTime TransactionDate, CancellationToken cancellationToken);
}
