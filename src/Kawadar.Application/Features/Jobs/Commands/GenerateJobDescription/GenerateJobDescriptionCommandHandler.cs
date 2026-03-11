using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Jobs.Commands.GenerateJobDescription;

public class GenerateJobDescriptionCommandHandler
    : IRequestHandler<GenerateJobDescriptionCommand, Result<GeneratedJobDescriptionDto>>
{
  private readonly IAIService _aiService;
  private readonly ILogger<GenerateJobDescriptionCommandHandler> _logger;

  public GenerateJobDescriptionCommandHandler(
      IAIService aiService,
      ILogger<GenerateJobDescriptionCommandHandler> logger)
  {
    _aiService = aiService;
    _logger = logger;
  }

  public async Task<Result<GeneratedJobDescriptionDto>> Handle(
      GenerateJobDescriptionCommand request,
      CancellationToken cancellationToken)
  {
    var prompt = $"""
            You are an expert job description writer.
            Based on the following context provided by the user, generate a clear, professional, and detailed job description.

            Rules:
            - The job description must clearly identify exactly what should be done.
            - The description must be written in the SAME LANGUAGE as the context provided below.
            - Be specific about responsibilities, requirements, and deliverables.
            - Use a professional tone appropriate for a freelance job posting.
            - Do NOT add information that is not implied or stated in the context.
            - Keep the description concise but comprehensive (150-400 words).

            User Context:
            {request.Context}
            """;

    _logger.LogInformation("Generating job description using AI service");

    var result = await _aiService.GenerateStructuredResponseAsync<GeneratedJobDescriptionDto>(
        prompt, cancellationToken);

    if (result.IsError)
    {
      _logger.LogWarning("AI service failed to generate job description: {Errors}", result.Errors);
      return result.Errors;
    }

    _logger.LogInformation("Successfully generated job description");
    return result.Value;
  }
}
