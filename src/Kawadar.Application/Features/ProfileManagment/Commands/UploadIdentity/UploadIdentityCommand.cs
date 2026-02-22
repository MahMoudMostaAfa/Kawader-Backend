using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;


public record UploadIdentityCommand(IFormFile FrontImage, IFormFile BackImage) : IRequest<Result<Success>>;