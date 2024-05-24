using Dapper;
using TravelAgency.FinancialService.API.Common.Models;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Infrastructure.Repositories;

public sealed class TaxRepository : ITaxRepository
{
    private readonly IFinancialServiceDbContext _context;

    public TaxRepository(IFinancialServiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaxDto>> ListWithTaxTypeAsync(DateTime TransactionDate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = _context.CreateConnection();

        var fees = await connection.QueryAsync<TaxDto>("SELECT * FROM GetTaxesByTransactionDate(@TransactionDate)", new { TransactionDate });

        return fees;
    }
}
