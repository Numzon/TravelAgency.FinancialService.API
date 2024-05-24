using DotNet.Testcontainers.Containers;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace TravelAgency.FinancialService.Tests.Shared.Configurations;
public class TestContainerConfiguration : IDisposable
{
    public IContainer MsSqlDatabase { get; private set; }
    public IContainer RabbitMqContainer { get; private set; }

    public TestContainerConfiguration()
    {
        MsSqlDatabase = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
            .WithName("SQLServerTests")
            .WithPortBinding(2100, 1433)
            .WithPassword("test.1234")
            .Build();

        RabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithName("RabbitMqTests")
            .WithPortBinding(15600, 15672)
            .WithPortBinding(5600, 5672)
            .WithUsername("admin")
            .WithPassword("test.1234")
            .Build();

        MsSqlDatabase.StartAsync().Wait();
        RabbitMqContainer.StartAsync().Wait();
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
            MsSqlDatabase.StopAsync().Wait();
            RabbitMqContainer.StopAsync().Wait();
        }
    }
}