using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using Ardalis.GuardClauses;
using TravelAgency.FinancialService.API.Common.Settings;
using TravelAgency.FinancialService.API.Infrastructure.Stored;

namespace TravelAgency.FinancialService.API.Infrastructure.Persistance;

public class FinancialServiceDbContextInitialiser
{
    private readonly FinancialServiceDbContext _context;
    private readonly DatabaseSettings _settings;

    public FinancialServiceDbContextInitialiser(FinancialServiceDbContext context, IOptions<DatabaseSettings> options)
    {
        _context = context;
        _settings = options.Value;
    }

    public async Task InitialiseAsync()
    {
        using (var defaultConnection = _context.CreateDefaultConnection())
        {
            Guard.Against.Null(defaultConnection);

            await InitialiseDatabaseAsync(defaultConnection);
        }

        using var connection = _context.CreateConnection();

        Guard.Against.Null(connection);

        await InitialiseTablesAsync(connection);
        await InitialiseProceduresAsync(connection);
        await InitialiseFunctionsAsync(connection);
    }

    public Task SeedAsync()
    {
        using var connection = _context.CreateConnection();

        return Task.CompletedTask;
    }

    private async Task InitialiseDatabaseAsync(IDbConnection connection)
    {
        var sql = $"SELECT COUNT(1) FROM sys.databases WHERE [name] = @Name;";

        var parameters = new DynamicParameters();
        parameters.Add("Name", _settings.InitialCatalog, DbType.String);

        var sqlDbCount = await connection.ExecuteScalarAsync<bool>(sql, parameters);

        if (sqlDbCount is false)
        {
            sql = $"CREATE DATABASE {_settings.InitialCatalog}";
            await connection.ExecuteAsync(sql);
        }
    }

    private async Task InitialiseTablesAsync(IDbConnection connection)
    {
        var sql = @$"{Tables.TaxType}
                    {Tables.ServiceFee}    
                    {Tables.FinancialReport}    
                    {Tables.Tax}
                    {Tables.TaxFinancialReport}";

        await connection.ExecuteAsync(sql);
    }

    private async Task InitialiseFunctionsAsync(IDbConnection connection)
    {
        await connection.ExecuteAsync(Functions.ServiceFeesByTransactionDate);
        await connection.ExecuteAsync(Functions.TaxesByTransactionDate);
    }

    private async Task InitialiseProceduresAsync(IDbConnection connection)
    {
        //inserts
        await connection.ExecuteAsync(Procedures.Insert.TaxType);
        await connection.ExecuteAsync(Procedures.Insert.Tax);
        await connection.ExecuteAsync(Procedures.Insert.ServiceFee);
        await connection.ExecuteAsync(Procedures.Insert.FinancialReport);
        await connection.ExecuteAsync(Procedures.Insert.TaxFinancialReport);

        //updates
        await connection.ExecuteAsync(Procedures.Update.ServiceFeeActiveTo);
    }
}
