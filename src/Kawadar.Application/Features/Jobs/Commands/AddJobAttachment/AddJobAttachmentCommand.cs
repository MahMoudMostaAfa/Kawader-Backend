using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobAttachment;

public record AddJobAttachmentCommand(
  string Slug,
  IFormFile? File,
  string? ExternalUrl
) : IRequest<Result<Created>>;
