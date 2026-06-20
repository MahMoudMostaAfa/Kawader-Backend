using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<RefreshTokenResponseDto>>;