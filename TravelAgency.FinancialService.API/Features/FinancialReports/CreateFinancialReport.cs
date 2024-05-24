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

public sealed class CreateFinancialReportHandler : IRequestHandler<CreateFinancialReportRequest, CreateFinancialReportResponse>
{
    private readonly IFinancialReportRepository _financialReportRepository;
    private readonly IServiceFeeRepository _serviceFeeRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly ITaxFinancialReportRepository _taxFinancialReportRepository;

    public CreateFinancialReportHandler(IFinancialReportRepository financialReportRepository, IServiceFeeRepository serviceFeeRepository, ITaxRepository taxRepository, ITaxFinancialReportRepository taxFinancialReportRepository)
    {
        _financialReportRepository = financialReportRepository;
        _serviceFeeRepository = serviceFeeRepository;
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
        var taxesWithTypes = await _taxRepository.ListWithTaxTypeAsync(oldestIncomeDate, cancellationToken);
        var taxTypes = taxesWithTypes.Select(x => x.TaxTypeId).Distinct();
        var taxFinancialReportDictionary = new Dictionary<int, TaxFinancialReportDto>();

        foreach (var item in incomes)
        {
            foreach (var type in taxTypes)
            {
                var tax = taxesWithTypes.First(x => type == x.TaxTypeId &&
                        (TaxSpecification.WasTransationMadeInOldTaxSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate)
                        || TaxSpecification.WasTransationMadeInLatestTaxSpan(x.ActiveFrom, x.ActiveTo, item.IncomeDate)));


                var cost = TaxSpecification.CountTax(item.Cost, tax.PercentValue);

                if (taxFinancialReportDictionary.ContainsKey(type))
                {
                    taxFinancialReportDictionary[type].AddToTaxCost(cost);
                }
                else
                {
                    taxFinancialReportDictionary.Add(type, new TaxFinancialReportDto(tax.TaxTypeName, financialReportId, cost));
                }
            }
        }

        return taxFinancialReportDictionary.Select(x => x.Value);
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