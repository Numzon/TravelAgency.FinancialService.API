namespace TravelAgency.FinancialService.API.Common.Models;

public sealed class ExpenseDto
{
    public required int Id { get; set; }
    public required decimal Cost { get; set; }
    public required DateTime ExpenseDate { get; set; }
}
