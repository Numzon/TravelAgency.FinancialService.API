using FastEndpoints;
using FluentValidation;
using MediatR;
using TravelAgency.FinancialService.API.Common.Models;
using TravelAgency.FinancialService.API.Domain.Specifications;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Features.FinancialReports;

public sealed record CreateFinancialReportResponse(int Id);

public sealed record CreateFinancialReportRequest(int TraveAgencyId, DateTime From, DateTime To, ICollection<IncomeDto> Incomes, ICollection<ExpenseDto>? Expenses) : IRequest<CreateFinancialReportResponse>;

public sealed class CreateFinancialReportEndpoint : Endpoint<CreateFinancialReportRequest, CreateFinancialReportResponse>
{
    private readonly ISender _sender;

    public CreateFinancialReportEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("/api/financial-reports");
        AllowAnonymous();
        Options(x => x.WithTags("Financial Reports"));
        Description(b => b
           .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json+problem")
           .ProducesProblemFE<InternalErrorResponse>(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(CreateFinancialReportRequest req, CancellationToken ct)
    {
        var response = await _sender.Send(req);

        await SendAsync(response, StatusCodes.Status201Created);
    }
}

public sealed class CreateTaxTypeHandler : IRequestHandler<CreateFinancialReportRequest, CreateFinancialReportResponse>
{
    private readonly IFinancialReportRepository _financialReportRepository;
    private readonly IServiceFeeRepository _serviceFeeRepository;
    private readonly ITaxTypeRepository _taxTypeRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly ITaxFinancialReportRepository _taxFinancialReportRepository;

    public CreateTaxTypeHandler(IFinancialReportRepository financialReportRepository, IServiceFeeRepository serviceFeeRepository, ITaxTypeRepository taxTypeRepository, ITaxRepository taxRepository, ITaxFinancialReportRepository taxFinancialReportRepository)
    {
        _financialReportRepository = financialReportRepository;
        _serviceFeeRepository = serviceFeeRepository;
        _taxTypeRepository = taxTypeRepository;
        _taxRepository = taxRepository;
        _taxFinancialReportRepository = taxFinancialReportRepository;
    }

    public async Task<CreateFinancialReportResponse> Handle(CreateFinancialReportRequest request, CancellationToken cancellationToken)
    {
        var totalServiceFee = await CountTotalServiceFeeAsync(request.Incomes, cancellationToken);
        var totalIncome = request.Incomes.Select(x => x.Cost).Sum();
        var totalExpense = request.Expenses?.Select(x => x.Cost).Sum() ?? 0;

        var report = new CreateFinancialReportDto(request.TraveAgencyId, totalIncome, totalExpense, totalServiceFee, request.From, request.To);

        var financialReportId = await _financialReportRepository.CreateAsync(report, cancellationToken);

        var taxes = await GetCountedTaxesAsync(request.Incomes, financialReportId, cancellationToken);
        
        await _taxFinancialReportRepository.CreateManyAsync(taxes, cancellationToken);  

        return new CreateFinancialReportResponse(financialReportId);
    }

    private async Task<decimal> CountTotalServiceFeeAsync(IEnumerable<IncomeDto> incomes, CancellationToken cancellationToken)
    {
        var oldestIncomeDate = GetOldestIncomeTransactionDate(incomes);
        var serviceFees = await _serviceFeeRepository.ListByTransactionDateAsync(oldestIncomeDate, cancellationToken);

        decimal serviceFeeCost = 0;

        foreach (var item in incomes)
        {
            var percent = serviceFees
                .Where(x => ServiceFeeSpecification.WasTransationMadeInOldServiceFeeSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate)
                    || ServiceFeeSpecification.WasTransationMadeInLatestServiceFeeSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate))
                .Select(x => x.PercentValue)
                .First();

            serviceFeeCost += ServiceFeeSpecification.CountServiceFee(item.Cost, percent);
        }

        return serviceFeeCost;
    }


    //refractor
    private async Task<IEnumerable<TaxFinancialReportDto>> GetCountedTaxesAsync(IEnumerable<IncomeDto> incomes, int financialReportId, CancellationToken cancellationToken)
    {
        var oldestIncomeDate = incomes.Select(x => x.IncomeDate).OrderBy(x => x).First();
        var tax = await _taxRepository.ListAsync(oldestIncomeDate, cancellationToken);
        var taxTypes = await _taxTypeRepository.ListAsync(tax.Select(x => x.TaxTypeId).Distinct(), cancellationToken);
        var typeAndSum = new Dictionary<int, decimal>();

        foreach (var item in incomes)
        {
            foreach (var type in taxTypes.Select(x => x.Id))
            {
                var percent = tax
                    .Where(x => type == x.TaxTypeId &&
                        (TaxSpecification.WasTransationMadeInOldTaxSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate)
                        || TaxSpecification.WasTransationMadeInLatestTaxSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate)))
                    .Select(x => x.PercentValue)
                    .First();


                if (typeAndSum.ContainsKey(type))
                {
                    typeAndSum[type] += TaxSpecification.CountTax(item.Cost, percent);
                }
                else
                {
                    typeAndSum.Add(type, TaxSpecification.CountTax(item.Cost, percent));
                }
            }
        }

        var taxFinancialReportList = new List<TaxFinancialReportDto>();
        foreach (var item in typeAndSum)
        {
            var type = taxTypes.FirstOrDefault(x => x.Id == item.Key);
            taxFinancialReportList.Add(new TaxFinancialReportDto(type!.Name, item.Value, financialReportId));
        }

        return taxFinancialReportList;
    }

    private DateTime GetOldestIncomeTransactionDate(IEnumerable<IncomeDto> incomes) 
    {
        return GetOldestTransactionDate(incomes.Select(x => x.IncomeDate));
    }

    private DateTime GetOldestTransactionDate(IEnumerable<DateTime> transactionDates)
    {
        return transactionDates.OrderBy(x => x).First();
    }
}

public sealed class CreateFinancialReportRequestValidator : Validator<CreateFinancialReportRequest>
{
    public CreateFinancialReportRequestValidator()
    {
        RuleFor(x => x.TraveAgencyId)
            .NotEmpty();

        RuleFor(x => x.From)
            .NotEmpty();

        RuleFor(x => x.To)
            .NotEmpty();

        RuleForEach(x => x.Incomes)
            .NotEmpty()
            .SetValidator(new IncomeDtoValidator());

        When(x => x.Expenses != null, () =>
        {
            RuleForEach(x => x.Expenses)
                .NotEmpty()
                .SetValidator(new ExpenseDtoValidator());
        });
    }
}

public sealed class IncomeDtoValidator : Validator<IncomeDto>
{
    public IncomeDtoValidator()
    {
        RuleFor(x => x.IncomeDate)
            .NotEmpty();

        RuleFor(x => x.Cost)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
public sealed class ExpenseDtoValidator : Validator<ExpenseDto>
{
    public ExpenseDtoValidator()
    {
        RuleFor(x => x.ExpenseDate)
            .NotEmpty();

        RuleFor(x => x.Cost)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}