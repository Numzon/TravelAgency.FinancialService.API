using FastEndpoints;
using FluentValidation;
using MediatR;
using TravelAgency.FinancialService.API.Infrastructure.Interfaces;

namespace TravelAgency.FinancialService.API.Features.TaxTypes;
public sealed record CreateTaxTypeResponse(int Id);

public sealed record CreateTaxTypeRequest(string Name) : IRequest<CreateTaxTypeResponse>;

public sealed class CreateTaxTypeEndpoint : Endpoint<CreateTaxTypeRequest, CreateTaxTypeResponse>
{
    private readonly ISender _sender;

    public CreateTaxTypeEndpoint(ISender  sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("/api/tax-types");
        AllowAnonymous();
        Options(x => x.WithTags("Tax Types"));
        Description(b => b
           .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json+problem") 
           .ProducesProblemFE<InternalErrorResponse>(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(CreateTaxTypeRequest req, CancellationToken ct)
    {
        var response = await _sender.Send(req);

        await SendAsync(response, StatusCodes.Status201Created);
    }
}
 
public sealed class CreateTaxTypeHandler : IRequestHandler<CreateTaxTypeRequest, CreateTaxTypeResponse>
{
    private readonly ITaxTypeRepository _repository;

    public CreateTaxTypeHandler(ITaxTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTaxTypeResponse> Handle(CreateTaxTypeRequest request, CancellationToken cancellationToken)
    {
        var taxTypeId = await _repository.CreateAsync(request, cancellationToken);

        return new CreateTaxTypeResponse(taxTypeId);
    }
}

public sealed class CreateTaxTypeRequestValidator : Validator<CreateTaxTypeRequest>
{
    public CreateTaxTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}