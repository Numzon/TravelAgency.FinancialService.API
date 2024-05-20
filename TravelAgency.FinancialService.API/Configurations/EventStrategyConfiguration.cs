using TravelAgency.SharedLibrary.RabbitMQ;

namespace TravelAgency.FinancialService.API.Configurations;

public static class EventStrategyConfiguration
{
    public static TypeEventStrategyConfig GetGlobalSettingsConfiguration()
    {
        var config = TypeEventStrategyConfig.GlobalSetting;

        //config.NewConfig<UserCreatedForEmployeeEventStrategy>(EventTypes.UserForEmployeeCreated);

        return config;
    }
}
