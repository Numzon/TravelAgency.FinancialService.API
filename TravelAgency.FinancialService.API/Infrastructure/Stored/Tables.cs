namespace TravelAgency.FinancialService.API.Infrastructure.Stored;

public static class Tables
{
    private const string AudiableProperties =
        @"Created DATETIME, 
            CreatedBy NVARCHAR(MAX), 
            LastModified DATETIME,
            LastModifiedBy NVARCHAR(MAX)";

    public const string TaxType =
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'TaxType')
            CREATE TABLE TaxType (
                Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                Name NVARCHAR(MAX) NOT NULL,
                {AudiableProperties}
            );";

    public const string ServiceFee =
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'ServiceFee')
                CREATE TABLE ServiceFee (
                    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                    PercentValue FLOAT NOT NULL,
                    ActiveFrom DATETIME NOT NULL,
                    ActiveTo DATETIME,
                    {AudiableProperties}
             );";

    public const string Tax =
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'Tax')
                    CREATE TABLE Tax (
                        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                        PercentValue FLOAT NOT NULL,
                        ActiveFrom DATETIME NOT NULL,
                        ActiveTo DATETIME,
                        TaxTypeId INT NOT NULL FOREIGN KEY REFERENCES TaxType(Id),
                        {AudiableProperties}
            );";

    public const string TaxFinancialReport =
    $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'TaxFinancialReport')
                    CREATE TABLE TaxFinancialReport (
                        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                        Name NVARCHAR(MAX) NOT NULL,
                        Cost MONEY NOT NULL,
                        FinancialReportId INT NOT NULL FOREIGN KEY REFERENCES FinancialReport(Id),
                        {AudiableProperties}
            );";

    public const string FinancialReport =
       $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'FinancialReport')
                    CREATE TABLE FinancialReport (
                        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                        TraveAgencyId INT NOT NULL,
                        AgencyIncome MONEY NOT NULL,
                        AgencyExpenses MONEY NOT NULL,
                        ServiceFeeCost MONEY NOT NULL,
                        DateRangeFrom DATETIME NOT NULL,
                        DateRangeTo DATETIME NOT NULL,
                        {AudiableProperties}
            );";
}
