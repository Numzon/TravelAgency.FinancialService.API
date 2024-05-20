namespace TravelAgency.FinancialService.API.Infrastructure.Stored;

public static class Procedures
{
    public static class Insert
    {
        public const string TaxType = @"
            CREATE OR ALTER PROCEDURE InsertTaxType @Name NVARCHAR(MAX), @Created DATETIME, @CreatedBy NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @TaxTypeIds TABLE (Id INT);    

	            INSERT INTO TaxType ([Name], Created, CreatedBy, LastModified, LastModifiedBy)
	            OUTPUT INSERTED.Id INTO @TaxTypeIds
	            VALUES (@Name, @Created, @CreatedBy, @Created, @CreatedBy);

	            SELECT * FROM @TaxTypeIds;
            END;";

        public const string Tax = @"
            CREATE OR ALTER PROCEDURE InsertTax @Percent FLOAT, @TaxTypeId INT, @ActiveFrom DATETIME, @Created DATETIME, @CreatedBy NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @TaxIds TABLE (Id INT);    

	            INSERT INTO Tax (PercentValue, ActiveFrom, TaxTypeId, Created, CreatedBy, LastModified, LastModifiedBy)
	            OUTPUT INSERTED.Id INTO @TaxIds
	            VALUES (@Percent, @ActiveFrom, @TaxTypeId, @Created, @CreatedBy, @Created, @CreatedBy);

	            SELECT * FROM @TaxIds;
            END;";

        public const string ServiceFee = @"
            CREATE OR ALTER PROCEDURE InsertServiceFee @Percent FLOAT, @ActiveFrom DATETIME, @Created DATETIME, @CreatedBy NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @ServiceFeeIds TABLE (Id INT);    

	            INSERT INTO ServiceFee (PercentValue, ActiveFrom, Created, CreatedBy, LastModified, LastModifiedBy)
	            OUTPUT INSERTED.Id INTO @ServiceFeeIds
	            VALUES (@Percent, @ActiveFrom, @Created, @CreatedBy, @Created, @CreatedBy);

	            SELECT * FROM @ServiceFeeIds;
            END;";

        public const string FinancialReport = @"
            CREATE OR ALTER PROCEDURE InsertFinancialReport @TravelAgencyId INT, @AgencyIncome MONEY, @AgencyExpenses MONEY, @DateRangeFrom DATETIME, @DateRangeTo DATETIME, @ServiceFeeCost MONEY, @Created DATETIME, @CreatedBy NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @FinancialReportIds TABLE (Id INT);    

	            INSERT INTO FinancialReport(TraveAgencyId, AgencyIncome, AgencyExpenses, DateRangeFrom, DateRangeTo, ServiceFeeCost,  Created, CreatedBy, LastModified, LastModifiedBy)
	            OUTPUT INSERTED.Id INTO @FinancialReportIds
	            VALUES (@TravelAgencyId, @AgencyIncome, @AgencyExpenses, @DateRangeFrom, @DateRangeTo, @ServiceFeeCost, @Created, @CreatedBy, @Created, @CreatedBy);

	            SELECT * FROM @FinancialReportIds;
            END;";

        public const string TaxFinancialReport = @"
            CREATE OR ALTER PROCEDURE InsertTaxFinancialReport @Name NVARCHAR(MAX), @Cost MONEY, @FinancialReportId INT, @Created DATETIME, @CreatedBy NVARCHAR(MAX)
            AS
            BEGIN
                DECLARE @TaxFinancialReportIds TABLE (Id INT);    

	            INSERT INTO TaxFinancialReport(Name, Cost, FinancialReportId, Created, CreatedBy, LastModified, LastModifiedBy)
	            OUTPUT INSERTED.Id INTO @TaxFinancialReportIds
	            VALUES (@Name, @Cost, @FinancialReportId, @Created, @CreatedBy, @Created, @CreatedBy);

	            SELECT * FROM @TaxFinancialReportIds;
            END;";
    }

    public static class Update
    {
        public const string ServiceFeeActiveTo = @"
            CREATE OR ALTER PROCEDURE UpdateServiceFeeActiveTo @Id INT, @ActiveTo DATETIME, @LastModified DATETIME, @LastModifiedBy NVARCHAR(MAX)
            AS
            BEGIN
	            UPDATE ServiceFee
	            SET ActiveTo = @ActiveTo,
                    LastModified = @LastModified, 
                    LastModifiedBy = @LastModifiedBy
	            WHERE Id = @Id;
            END;";
    }
}
