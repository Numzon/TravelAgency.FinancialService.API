using TravelAgency.FinancialService.API.Domain.Common;

namespace TravelAgency.FinancialService.API.Domain.Entities;

public sealed class Tax : BaseAuditableEntity
{
    public required double PercentValue { get; set; }
    public required DateTime ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }

    public required int TaxTypeId { get; set; }
    public required TaxType TaxType { get; set; }
}
