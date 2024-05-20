using Dapper;
using System.Data;
using TravelAgency.FinancialService.API.Common.Models;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Infrastructure.Repositories;

public sealed class TaxFinancialReportRepository : ITaxFinancialReportRepository
{
    private readonly IFinancialServiceDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public TaxFinancialReportRepository(IFinancialServiceDbContext context, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public async Task CreateManyAsync(IEnumerable<TaxFinancialReportDto> taxFinancialReportList, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = _context.CreateConnection();

        var rows = new List<object>();

        foreach (var item in taxFinancialReportList)
        {
            rows.Add(new { item.Name, item.Cost, item.FinancialReportId, Created = _dateTimeService.Now, CreatedBy = _currentUserService.Id });
        }

        await connection.ExecuteAsync("[dbo].[InsertTaxFinancialReport]", rows, commandType: CommandType.StoredProcedure);
    }
}
