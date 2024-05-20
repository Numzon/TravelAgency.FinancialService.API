using TravelAgency.FinancialService.API.Common.Models;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface IFinancialReportRepository
{
    Task<int> CreateAsync(CreateFinancialReportDto report, CancellationToken cancellationToken);
}
