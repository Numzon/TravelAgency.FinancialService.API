using Dapper;
using System.Data;
using TravelAgency.FinancialService.API.Common.Models;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Infrastructure.Repositories;

public sealed class FinancialReportRepository : IFinancialReportRepository
{
    private readonly IFinancialServiceDbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public FinancialReportRepository(IFinancialServiceDbContext context, IDateTimeService dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<int> CreateAsync(CreateFinancialReportDto report, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("TravelAgencyId", report.TravelAgencyId);
        parameters.Add("AgencyIncome", report.AgencyIncome);
        parameters.Add("AgencyExpenses", report.AgencyExpenses);
        parameters.Add("DateRangeFrom", report.From);
        parameters.Add("DateRangeTo", report.To);
        parameters.Add("ServiceFeeCost", report.ServiceFeeCost);
        parameters.Add("Created", _dateTimeService.Now);
        parameters.Add("CreatedBy", _currentUserService.Id);

        var id = await connection.ExecuteScalarAsync<int>("[dbo].[InsertFinancialReport]", parameters, commandType: CommandType.StoredProcedure);

        return id;
    }
}
