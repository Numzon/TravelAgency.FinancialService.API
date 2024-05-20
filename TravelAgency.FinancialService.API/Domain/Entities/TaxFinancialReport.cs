using TravelAgency.FinancialService.API.Domain.Common;

namespace TravelAgency.FinancialService.API.Domain.Entities;

public sealed class TaxFinancialReport : BaseAuditableEntity
{
    public required string Name { get; set; }
    public required decimal Cost { get; set; }

    public required int FinancialReportId { get; set; }
    public required FinancialReport FinancialReport { get; set; }
}
