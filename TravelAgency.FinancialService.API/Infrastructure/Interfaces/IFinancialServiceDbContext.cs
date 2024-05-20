using System.Data;

namespace TravelAgency.FinancialService.API.Infrastructure.Interfaces;

public interface IFinancialServiceDbContext
{
    IDbConnection CreateConnection();
}
