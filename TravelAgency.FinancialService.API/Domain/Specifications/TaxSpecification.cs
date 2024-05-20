namespace TravelAgency.FinancialService.API.Domain.Specifications;

public static class TaxSpecification
{
    public static bool WasTransationMadeInOldTaxSpan(DateTime activeFrom, DateTime? activeTo, DateTime transactionDate)
    {
        if (activeTo.HasValue is false)
        {
            return false;
        }

        return activeFrom <= transactionDate && activeTo >= transactionDate;
    }

    public static bool WasTransationMadeInLatestTaxSpan(DateTime activeFrom, DateTime? activeTo, DateTime transactionDate)
    {

        return activeFrom <= transactionDate && activeTo == null;
    }

    public static decimal CountTax(decimal income, double percent)
    {
        return income * Convert.ToDecimal(percent) / 100;
    }
}
