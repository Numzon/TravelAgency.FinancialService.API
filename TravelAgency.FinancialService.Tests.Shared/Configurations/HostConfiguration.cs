using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelAgency.FinancialService.Tests.Shared.Authentication;

namespace TravelAgency.FinancialService.Tests.Shared.Configurations;
public static class HostConfiguration
{
    public static WebApplicationFactory<Program> Build()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseContentRoot(".")
                    .UseEnvironment("test")
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    })
                    .ConfigureTestServices(services =>
                    {
                        services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = "1");

                        services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                            .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
                    });
            });

    }
}
