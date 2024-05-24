using Dapper;
using TravelAgency.FinancialService.API.Domain.Entities;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.FinancialService.Tests.Shared.Interfaces;

namespace TravelAgency.FinancialService.API.IntegrationTests.PrepopulateStrategies;


public class CreateFinancialReportPrepopulationStrategy : IPrepopulateStrategy<IFinancialServiceDbContext>
{
    private const string VatTaxTypeName = "VAT";
    private const string CitTaxTypeName = "CIT";

    public async Task PrepopulateAsync(IFinancialServiceDbContext context, IFixture fixture)
    {
        using var connection = context.CreateConnection();

        var fees = GetPreparedServiceFees();
        await connection.ExecuteAsync("INSERT INTO dbo.ServiceFee (PercentValue, ActiveFrom, ActiveTo) VALUES (@PercentValue, @ActiveFrom, @ActiveTo);", fees);

        var taxTypes = GetPreparedTaxTypes();
        await connection.ExecuteAsync("INSERT INTO dbo.TaxType (Name) VALUES (@Name);", taxTypes);

        var taxTypesFromDb = await connection.QueryAsync<TaxType>("SELECT * FROM dbo.TaxType");

        var vatId = taxTypesFromDb.First(x => x.Name == VatTaxTypeName).Id;
        var citId = taxTypesFromDb.First(x => x.Name == CitTaxTypeName).Id;

        var taxes = GetPreparedTaxes(vatId, citId);
        await connection.ExecuteAsync("INSERT INTO dbo.Tax (TaxTypeId, PercentValue, ActiveFrom, ActiveTo) VALUES (@TaxTypeId, @PercentValue, @ActiveFrom, @ActiveTo)", taxes);
    }

    private IEnumerable<ServiceFee> GetPreparedServiceFees()
    {
        return new List<ServiceFee>() {
              new ()
              {
                  PercentValue = 2,
                  ActiveFrom = new DateTime(2024,03,1),
                  ActiveTo = new DateTime(2024,03,31)
              },
              new(){
                  PercentValue = 7,
                  ActiveFrom = new DateTime(2024,04,1),
                  ActiveTo = new DateTime(2024,04,30)
              },
              new()
              {
                  PercentValue = 4,
                  ActiveFrom = new DateTime(2024,05,1)
              }
        };
    }

    private ICollection<TaxType> GetPreparedTaxTypes()
    {
        return new List<TaxType>() { new() { Name = VatTaxTypeName }, new() { Name = CitTaxTypeName } };
    }

    private object[] GetPreparedTaxes(int vatTaxTypeId, int citTaxTypeId)
    {
        DateTime? activeTo = new DateTime(2024, 04, 30);
        DateTime? nullActiveTo = null!;

        return new []
        {
            new {
                TaxTypeId = vatTaxTypeId,
                PercentValue = 18,
                ActiveFrom = new DateTime(2024, 04, 01),
                ActiveTo = activeTo
            },
            new {
                TaxTypeId = vatTaxTypeId,
                PercentValue = 23,
                ActiveFrom = new DateTime(2024, 05, 01),
                ActiveTo = nullActiveTo
            },
            new {
                TaxTypeId = citTaxTypeId,
                PercentValue = 5,
                ActiveFrom = new DateTime(2024, 04, 01),
                ActiveTo = nullActiveTo
            }
        };
    }
}
