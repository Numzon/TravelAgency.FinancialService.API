namespace TravelAgency.FinancialService.API.Common.Models;

public sealed class TaxFinancialReportDto
{
    private decimal _cost;

    public string Name { get; private set; }
    public int FinancialReportId { get; private set; }
    public decimal Cost => _cost;

    public TaxFinancialReportDto(string name, int financialReportId, decimal cost)
    {
        Name = name;
        FinancialReportId = financialReportId;
        _cost = cost;
    }

    public void AddToTaxCost(decimal cost)
    {
        _cost += cost;
    }
}
