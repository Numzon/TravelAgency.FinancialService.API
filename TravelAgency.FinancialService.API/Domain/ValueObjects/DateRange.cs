namespace TravelAgency.FinancialService.API.Domain.ValueObjects;

public struct DateRange
{
    public required DateTime From { get; set; }
    public required DateTime To { get; set; }
}
