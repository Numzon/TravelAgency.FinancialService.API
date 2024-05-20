namespace TravelAgency.FinancialService.API.Infrastructure.Stored;

public static class Functions
{
    public const string ServiceFeesByTransactionDate = @"
        CREATE OR ALTER FUNCTION GetServiceFeesByTransactionDate (@TransactionDate DATETIME)
        RETURNS TABLE
        AS
        RETURN
            SELECT * FROM ServiceFee
            WHERE (ActiveFrom <= @TransactionDate and ActiveTo is not null and ActiveTo >= @TransactionDate)
		        or (ActiveFrom <= @TransactionDate and ActiveTo is null)
		        or (ActiveFrom >= @TransactionDate);";

    public const string TaxesByTransactionDate = @"
        CREATE OR ALTER FUNCTION GetTaxesByTransactionDate (@TransactionDate DATETIME)
        RETURNS TABLE
        AS
        RETURN
            SELECT * FROM Tax
            WHERE (ActiveFrom <= @TransactionDate and ActiveTo is not null and ActiveTo >= @TransactionDate)
		        or (ActiveFrom <= @TransactionDate and ActiveTo is null)
		        or (ActiveFrom >= @TransactionDate);";
}
