using TravelAgency.FinancialService.API.Domain.Common;
using TravelAgency.FinancialService.API.Domain.ValueObjects;

namespace TravelAgency.FinancialService.API.Domain.Entities;

public sealed class FinancialReport : BaseAuditableEntity
{
    public required int TraveAgencyId { get; set; }
    public required decimal AgencyIncome { get; set; } 
    public required decimal AgencyExpenses { get; set; }
    public required decimal ServiceFeeCost { get; set; }
    public required DateRange DateRange { get; set; }

    public required ICollection<TaxFinancialReport> Taxes { get; set; }
}

