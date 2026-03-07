using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.GenerateJobDescription;

public record GenerateJobDescriptionCommand(
    string Context
) : IRequest<Result<GeneratedJobDescriptionDto>>;
