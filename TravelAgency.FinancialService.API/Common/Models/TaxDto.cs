namespace TravelAgency.FinancialService.API.Common.Models;

public sealed class TaxDto
{
    public required int Id { get; set; }
    public required double PercentValue { get; set; }
    public required DateTime ActiveFrom { get; set; }
    public required DateTime? ActiveTo { get; set; }
    public required int TaxTypeId { get; set; }
    public required string TaxTypeName { get; set; }
}
