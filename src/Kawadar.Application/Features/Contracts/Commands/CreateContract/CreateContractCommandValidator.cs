using FluentValidation;
using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Application.Features.Contracts.Commands.CreateContract;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
  public CreateContractCommandValidator()
  {
    RuleFor(c => c.JobId)
      .NotEmpty().WithMessage("JobId is required.");

    RuleFor(c => c.ProposaslId)
      .NotEmpty().WithMessage("FreelancerId is required.");




    RuleFor(c => c.ContractType).IsInEnum().WithMessage("Invalid ContractType.");



  }
}