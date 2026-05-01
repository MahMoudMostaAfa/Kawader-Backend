using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Commands.RequestContractCompletion;

public class RequestContractCompletionCommandValidator : AbstractValidator<RequesContractCompletionCommand>
{
  public RequestContractCompletionCommandValidator()
  {
    RuleFor(x => x.ContractId)
        .NotEmpty().WithMessage("Contract Id is required.");

  }
}