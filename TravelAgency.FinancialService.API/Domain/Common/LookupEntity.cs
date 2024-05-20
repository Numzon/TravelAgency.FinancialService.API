namespace TravelAgency.FinancialService.API.Domain.Common;

public abstract class LookupEntity : BaseAuditableEntity
{
    public required string Name { get; set; }
}
