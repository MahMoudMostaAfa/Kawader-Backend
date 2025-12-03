using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;
namespace Kawadar.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password) : IRequest<Result<Success>>;