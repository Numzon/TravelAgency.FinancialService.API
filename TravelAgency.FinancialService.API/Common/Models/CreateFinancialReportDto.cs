namespace TravelAgency.FinancialService.API.Common.Models;

public sealed record CreateFinancialReportDto(int TravelAgencyId, decimal AgencyIncome, decimal AgencyExpenses, decimal ServiceFeeCost, DateTime From, DateTime To);
