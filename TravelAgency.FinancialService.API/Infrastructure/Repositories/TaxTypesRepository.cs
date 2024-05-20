using TravelAgency.FinancialService.API.Features.TaxTypes;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using Dapper;
using System.Data;
using TravelAgency.FinancialService.API.Domain.Entities;
using Mapster;

namespace TravelAgency.FinancialService.API.Infrastructure.Repositories;

public sealed class TaxTypesRepository : ITaxTypeRepository
{
    private readonly IFinancialServiceDbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public TaxTypesRepository(IFinancialServiceDbContext context, IDateTimeService dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<int> CreateAsync(CreateTaxTypeRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();   

        using (var dbConnection = _context.CreateConnection())
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", request.Name);
            parameters.Add("Created", _dateTimeService.Now);
            parameters.Add("CreatedBy", _currentUserService.Id);

            var id = await dbConnection.ExecuteScalarAsync<int>("[dbo].[InsertTaxType]", parameters, commandType: CommandType.StoredProcedure);

            return id;
        }
    }

    public async Task<IEnumerable<TaxType>> ListAsync(IEnumerable<int> taxTypeIds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var dbConnection = _context.CreateConnection();

        var types = await dbConnection.QueryAsync<TaxType>("SELECT * FROM TaxType WHERE Id IN @Ids;", new { Ids = taxTypeIds.ToArray() });

        return types;
    }
}
