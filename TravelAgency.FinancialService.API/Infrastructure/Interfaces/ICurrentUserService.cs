namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface ICurrentUserService
{
    string? AccessToken { get; }
    string? Id { get; }
}
