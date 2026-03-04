using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJobAttachment;

public record DeleteJobAttachmentCommand(
  string Slug,
  Guid AttachmentId
) : IRequest<Result<Deleted>>;
