using Microsoft.AspNetCore.Authentication;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;
using TravelAgency.SharedLibrary.Enums;

namespace TravelAgency.FinancialService.API.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly string? _accessToken;
    private readonly string? _id;

    public CurrentUserService(IHttpContextAccessor context)
    {
        _accessToken = context.HttpContext?.GetTokenAsync(AwsTokenNames.AccessToken).Result;
        _id = context.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == AwsTokenNames.Username)?.Value;
    }

    public string? AccessToken => _accessToken;
    public string? Id => _id;
}
