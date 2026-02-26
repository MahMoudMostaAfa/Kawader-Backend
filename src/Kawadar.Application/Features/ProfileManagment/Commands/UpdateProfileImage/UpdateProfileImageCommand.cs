using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfileImage;

public record UpdateProfileImageCommand(IFormFile ProfilePic) : IRequest<Result<Updated>>;
