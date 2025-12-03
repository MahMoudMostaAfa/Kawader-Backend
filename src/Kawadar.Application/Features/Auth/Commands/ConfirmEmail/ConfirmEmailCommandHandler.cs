using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<Success>>
{

  private readonly IIdentityService _identityService;
  public ConfirmEmailCommandHandler(IIdentityService identityService)
  {
    _identityService = identityService;
  }
  public async Task<Result<Success>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
  {
    var filteredToken = Uri.UnescapeDataString(request.Token);
    var identifyResult = await _identityService.ConfirmEmailAsync(request.UserId, filteredToken);
    if (identifyResult.IsError) return identifyResult.Errors;

    return Result.Success;
  }
}