using TravelAgency.FinancialService.Tests.Shared.Configurations;
using TravelAgency.FinancialService.Tests.Shared.Enums;

namespace TravelAgency.FinancialService.API.IntegrationTests.Configurations;
[CollectionDefinition(CollectionDefinitions.IntergrationTestCollection)]
public class IntegrationTestCollection : ICollectionFixture<TestContainerConfiguration>
{
    //configuration, no code here, class will not be ever created
}
