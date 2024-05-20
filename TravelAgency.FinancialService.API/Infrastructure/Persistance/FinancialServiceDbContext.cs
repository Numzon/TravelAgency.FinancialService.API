using Ardalis.GuardClauses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using TravelAgency.FinancialService.API.Common.Settings;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Infrastructure.Persistance;

public class FinancialServiceDbContext : IFinancialServiceDbContext
{
    private readonly string _connectionString;
    private readonly string _defaultConnectionString;

    public FinancialServiceDbContext(IOptions<DatabaseSettings> options)
    {
        var settings = options.Value;

        Guard.Against.Null(settings);

        _connectionString = BuildConnectionString(settings);
        _defaultConnectionString = BuildDefaultConnectionString(settings);
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    /// <summary>
    /// Builds connection string to postgresSQL with default database name set as 'postgres'
    /// Made to handle many databases in one postgres container - should be used only in initialisation of database
    /// </summary>
    /// <returns>Connection string to default database</returns>
    public IDbConnection CreateDefaultConnection() => new SqlConnection(_defaultConnectionString);

    private string BuildConnectionString(DatabaseSettings settings)
    {
        return BuildConnectionStringWithGivenInitialCatalog(settings, settings.InitialCatalog);
    }
    private string BuildDefaultConnectionString(DatabaseSettings settings)
    {
        return BuildConnectionStringWithGivenInitialCatalog(settings, "master");
    }

    private string BuildConnectionStringWithGivenInitialCatalog(DatabaseSettings settings, string databaseName)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            { "User ID", settings.UserId },
            { "Password", settings.Password },
            { "Data Source", settings.DataSource },
            { "Initial Catalog", databaseName },
            { "TrustServerCertificate", settings.TrustServerCertificate }
        };

        return connectionStringBuilder.ToString();
    }
}
