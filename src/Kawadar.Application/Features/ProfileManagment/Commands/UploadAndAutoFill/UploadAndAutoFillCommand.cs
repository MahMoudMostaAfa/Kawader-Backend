using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadAndAutoFill;


public record UploadAndAutoFillCommand(IFormFile File) : IRequest<Result<ProfileAutoFillDto>>;