using Dapper;
using TravelAgency.FinancialService.API.Domain.Entities;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Infrastructure.Repositories;

public sealed class ServiceFeeRepository : IServiceFeeRepository
{
    private readonly IFinancialServiceDbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public ServiceFeeRepository(IFinancialServiceDbContext context, IDateTimeService dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<ServiceFee>> ListByTransactionDateAsync(DateTime TransactionDate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = _context.CreateConnection();

        var fees = await connection.QueryAsync<ServiceFee>("SELECT * FROM GetServiceFeesByTransactionDate(@TransactionDate)", new { TransactionDate });

        return fees;
    }
}
