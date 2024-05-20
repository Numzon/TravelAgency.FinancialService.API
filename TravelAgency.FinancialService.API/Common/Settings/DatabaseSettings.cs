namespace TravelAgency.FinancialService.API.Common.Settings;

public sealed class DatabaseSettings
{
    public required string UserId { get; set; }
    public required string Password { get; set; }
    public required string DataSource { get; set; }
    public required string InitialCatalog { get; set; }
    public required string TrustServerCertificate { get; set; }
}
