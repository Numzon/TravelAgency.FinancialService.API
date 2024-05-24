using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Net.Mime;
using System.Text;
using TravelAgency.FinancialService.API.Common.Models;
using TravelAgency.FinancialService.API.Domain.Entities;
using TravelAgency.FinancialService.API.Features.FinancialReports;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.API.IntegrationTests.PrepopulateStrategies;
using TravelAgency.FinancialService.Tests.Shared.Configurations;
using TravelAgency.FinancialService.Tests.Shared.Enums;

namespace TravelAgency.FinancialService.API.Tests.Features.FinancialReports;
[Collection(CollectionDefinitions.IntergrationTestCollection)]
public class CreateFinancialReportTests : BaseIntegrationTest
{
    [Fact]
    public async Task Create_ValidTypeName_CreatesAndReturnsNotificationType()
    {
        using (TestServer = HostConfiguration.Build().Server)
        {
            //Initialisation
            var strategy = new CreateFinancialReportPrepopulationStrategy();
            await InitializeDatabaseAsync(strategy);

            //Arrange
            var request = GetPreparedFinancialRequest();
            using HttpClient httpClient = TestServer.CreateClient();

            //Act
            HttpResponseMessage httpResponse = await httpClient.PostAsync($"/api/financial-reports",
                new StringContent(JsonConvert.SerializeObject(request, new StringEnumConverter()), Encoding.UTF8, MediaTypeNames.Application.Json));

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CreateFinancialReportResponse>(content);

            response.Should().NotBeNull();
            response!.Id.Should().BeGreaterThan(0);

            var context = TestServer.Services.GetRequiredService<IFinancialServiceDbContext>();

            using var connection = context.CreateConnection();
            var financialReports = await connection.QueryAsync<FinancialReport>("SELECT * FROM dbo.FinancialReport WHERE Id = @Id", new { response.Id });
            var taxes = await connection.QueryAsync<TaxFinancialReport>("SELECT * FROM dbo.TaxFinancialReport WHERE FinancialReportId = @FinancialReportId", new { FinancialReportId = response.Id });

            var financialReport = financialReports.First();

            financialReport.AgencyIncome.Should().Be(190);
            financialReport.AgencyExpenses.Should().Be(50);
            financialReport.ServiceFeeCost.Should().Be(12.10m);

            var vatTax = taxes.FirstOrDefault(x => x.Name == "VAT");
            var citTax = taxes.FirstOrDefault(x => x.Name == "CIT");

            vatTax.Should().NotBeNull();
            vatTax!.Cost.Should().Be(36.2m);

            citTax.Should().NotBeNull();
            citTax!.Cost.Should().Be(9.5m);

            //Cleanup
            await ResetDatabaseAsync();
        }
    }




    private ICollection<IncomeDto> GetPreparedIncomes()
    {
        return new List<IncomeDto>()
        {
            new()
            {
                Id = 1,
                Cost = 100,
                IncomeDate = new DateTime(2024,04,22)
            },
            new()
            {
                Id = 2,
                Cost = 50,
                IncomeDate = new DateTime(2024, 04,27)
            },
            new()
            {
                Id = 3,
                Cost = 40,
                IncomeDate = new DateTime(2024, 05, 1)
            }
        };
    }
    private ICollection<ExpenseDto> GetPreparedExpenses()
    {
        return new List<ExpenseDto>()
        {
            new()
            {
                Id = 1,
                Cost = 20,
                ExpenseDate = new DateTime(2024,04,20)
            },
            new()
            {
                Id = 2,
                Cost = 30,
                ExpenseDate = new DateTime(2024, 04,27)
            }
        };
    }

    private CreateFinancialReportRequest GetPreparedFinancialRequest()
    {
        return new CreateFinancialReportRequest(1, new DateTime(2024, 04, 01), new DateTime(2024, 05, 01), GetPreparedIncomes(), GetPreparedExpenses());
    }

}

