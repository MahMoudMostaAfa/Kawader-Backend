using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;
namespace Kawadar.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password, ProfileType ProfileType) : IRequest<Result<Success>>;