Vertical Slice Architecture
MediatR
Dapper + MsSql
StyleCop
Serilog
RabbitMQ
Mapster
FastEndpoints


public sealed class FinancialReport : BaseAuditableEntity
{
    public int TraveAgencyId { get; set; }
    public required DateTime From { get; set; }
    public required DateTime To { get; set; }
    public decimal AgencyIncome { get; set; } //with VAT
    public decimal AgencyExpenses { get; set; } //with VAT

    //taxes (all) as a list

    public decimal ServiceFeeCost { get; set; }
}


//Expenses are an endpoint, same payments, - they are based on date from - to (async multiple simple lists to make it work faster)
// expenses have vat tax in it and a type (expensetype that defines amount of VAT they can remove)(specification in his service)
//income is simpler - just income with 23% vat
//financial panel calculates tax (income, zus, vat) after button - generate raport - taxes are calculated here in financial service
//also there is service fee that is defined as a percent from income 

//table service fee holds latest percent (it stores date of creation, every income with date older than new one have old tax etc)


//income from work
//expenses
//cit tax
//vat tax
//what else tax

//service fee for that time
//result 


RULE #1 
Only the last added service fee that hasn't been activated yet (active from) can be deleted 
RULE #2
ActiveTo field is one day smaller than new activeFrom
RULE #3 
We only calculate tax from income value, so if we have VAT that is base VAT in income, expenses are not involved