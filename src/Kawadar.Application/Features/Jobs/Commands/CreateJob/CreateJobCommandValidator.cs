using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Jobs.Commands.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
        private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB

        public CreateJobCommandValidator()
        {
                RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
                RuleFor(x => x.SpecilizationId).NotEqual(Guid.Empty);
                RuleFor(x => x.DurationInDays).GreaterThan(0).LessThanOrEqualTo(365);

                RuleFor(x => x).Must(x => x.QuestionDtos == null || x.QuestionDtos.Count <= 5)
                        .WithMessage("A job can have a maximum of 5 questions.");

                RuleForEach(x => x.QuestionDtos).ChildRules(question =>
                {
                        question.RuleFor(q => q.Question).NotEmpty().MaximumLength(1000);
                });

                RuleFor(x => x)
                        .Must(x => (x.AttachmentFiles?.Count ?? 0) + (x.AttachmentLinks?.Count ?? 0) <= 5)
                        .WithMessage("A job can have a maximum of 5 attachments in total.");

                RuleFor(x => x).Must(x => x.SkillIds == null || x.SkillIds.Count <= 10)
                        .WithMessage("A job can have a maximum of 10 skills.");

                RuleFor(x => x.JobType).IsInEnum();
                RuleFor(x => x.BudgetRange).IsInEnum();
                RuleFor(x => x.HourlyRateRange).IsInEnum();
                RuleFor(x => x.ExperienceLevel).IsInEnum();




                RuleForEach(x => x.AttachmentFiles).ChildRules(file =>
                {
                        file.RuleFor(f => f.Length)
                    .LessThanOrEqualTo(MaxFileSizeBytes)
                    .WithMessage($"Attachment file size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB.");

                        file.RuleFor(f => f.FileName)
                    .Must(name => ExtensionValidator.ValidExtension(name, Extensions.AllowedJobAttachmentExtensions))
                    .WithMessage($"Allowed file types: {string.Join(", ", Extensions.AllowedJobAttachmentExtensions)}.");
                });

                RuleForEach(x => x.AttachmentLinks).ChildRules(link =>
                {
                        link.RuleFor(url => url)
                    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                    .WithMessage("External URL must be a valid absolute URL.");
                });

                When(X => X.IsPrivate, () =>
                {
                        RuleFor(x => x.PrivateToUserId).NotNull().NotEqual(Guid.Empty).WithMessage("PrivateToUserId must be provided and valid when IsPrivate is true.");
                });
        }
}
