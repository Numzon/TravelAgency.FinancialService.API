using TravelAgency.FinancialService.API.Domain.Entities;
using TravelAgency.FinancialService.API.Features.TaxTypes;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface ITaxTypeRepository
{
    Task<int> CreateAsync(CreateTaxTypeRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<TaxType>> ListAsync(IEnumerable<int> taxTypeIds, CancellationToken cancellationToken);
}
