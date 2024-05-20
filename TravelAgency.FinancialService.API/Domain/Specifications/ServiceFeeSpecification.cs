namespace TravelAgency.FinancialService.API.Domain.Specifications;

public static class ServiceFeeSpecification
{
    public static bool WasTransationMadeInOldServiceFeeSpan(DateTime activeFrom, DateTime? activeTo, DateTime transactionDate)
    {
        if (activeTo.HasValue is false)
        {
            return false;
        }

        return activeFrom <= transactionDate && activeTo >= transactionDate;
    }

    public static bool WasTransationMadeInLatestServiceFeeSpan(DateTime activeFrom, DateTime? activeTo, DateTime transactionDate)
    {

        return activeFrom <= transactionDate && activeTo == null;
    }

    public static decimal CountServiceFee(decimal income, double percent)
    {
        return income * Convert.ToDecimal(percent) / 100;
    }
}
