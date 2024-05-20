namespace TravelAgency.FinancialService.API.Common.Models;

public class IncomeDto
{
    public required int Id { get; set; }
    public required decimal Cost { get; set; }
    public required DateTime IncomeDate { get; set; }
}
