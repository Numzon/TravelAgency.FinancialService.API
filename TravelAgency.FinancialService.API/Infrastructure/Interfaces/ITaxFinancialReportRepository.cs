using TravelAgency.FinancialService.API.Common.Models;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface ITaxFinancialReportRepository
{
    Task CreateManyAsync(IEnumerable<TaxFinancialReportDto> taxFinancialReportList, CancellationToken cancellationToken);
}
