using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;
namespace Kawadar.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string name) : IRequest<Result<UserDto>>;