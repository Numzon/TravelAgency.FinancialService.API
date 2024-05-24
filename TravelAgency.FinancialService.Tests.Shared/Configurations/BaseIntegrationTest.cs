using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Ardalis.GuardClauses;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.API.Infrastructure.Persistance;
using TravelAgency.FinancialService.Tests.Shared.Interfaces;

namespace TravelAgency.FinancialService.Tests.Shared.Configurations;
public abstract class BaseIntegrationTest : IDisposable
{
    protected TestServer TestServer { get; set; } = null!;
    protected IFixture Fixture { get; private set; }

    private Respawner _respawner = null!;


    protected BaseIntegrationTest()
    {
        Fixture = new Fixture();

        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
         .ToList().ForEach(behavior => Fixture.Behaviors.Remove(behavior));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    protected async Task InitializeDatabaseAsync(IPrepopulateStrategy<IFinancialServiceDbContext>? strategy = null)
    {
        IServiceProvider serviceProvider = TestServer.Services;

        using IServiceScope scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetService<IFinancialServiceDbContext>();

        Guard.Against.Null(context);

        var initialiser = scope.ServiceProvider.GetService<FinancialServiceDbContextInitialiser>();

        Guard.Against.Null(initialiser);

        await initialiser.InitialiseAsync();

        var connection = context.CreateConnection();

        if (strategy is not null)
        {
            await strategy.PrepopulateAsync(context, Fixture);
        }

        var dbConnection = connection as DbConnection;

        Guard.Against.Null(dbConnection);

        await dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer
        });

        await dbConnection.CloseAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using IServiceScope scope = TestServer.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<IFinancialServiceDbContext>();

        Guard.Against.Null(context);

        var connection = context.CreateConnection();

        var dbConnection = connection as DbConnection;

        Guard.Against.Null(dbConnection);

        await dbConnection.OpenAsync();
        await _respawner.ResetAsync(dbConnection);
        await dbConnection.CloseAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);

    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TestServer.Dispose();
        }
    }
}