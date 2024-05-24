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
            SELECT t.Id, t.PercentValue, t.ActiveFrom, t.ActiveTo, tt.Id AS TaxTypeId, tt.Name AS TaxTypeName
			FROM Tax AS t
			JOIN TaxType AS tt ON t.TaxTypeId = tt.Id
            WHERE (t.ActiveFrom <= @TransactionDate and t.ActiveTo is not null and t.ActiveTo >= @TransactionDate)
		        or (t.ActiveFrom <= @TransactionDate and t.ActiveTo is null)
		        or (t.ActiveFrom >= @TransactionDate);";
}
