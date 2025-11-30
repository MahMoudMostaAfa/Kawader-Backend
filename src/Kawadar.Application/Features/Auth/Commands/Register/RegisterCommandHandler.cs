using AutoMapper;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IMapper mapper) : IRequestHandler<RegisterCommand, Result<UserDto>>
{
  public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
  {

    await Task.Delay(300, cancellationToken);

    return mapper.Map<UserDto>(request.name);
  }
}