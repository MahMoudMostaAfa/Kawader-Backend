using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJob;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.");

        // when updating, all fields are optional, but if provided, they should meet certain criteria
        RuleFor(x => x.Title)
            .MinimumLength(5)
             .MaximumLength(100)
             .When(x => !string.IsNullOrEmpty(x.Title))
             .WithMessage("Title must be between 5 and 100 characters.");


        RuleFor(x => x.Description)
        .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description)).WithMessage("Description must not exceed 2000 characters.");


        RuleFor(x => x.DurationInDays)
            .GreaterThan(0).When(x => x.DurationInDays.HasValue).WithMessage("Duration in days must be greater than 0.");


        RuleFor(x => x.SpecilizationId)
            .NotEmpty().When(x => x.SpecilizationId.HasValue).WithMessage("Specialization ID must be provided if specified.");

        RuleFor(x => x.JobType)
            .IsInEnum().When(x => x.JobType.HasValue).WithMessage("Invalid job type.");

        RuleFor(x => x.BudgetRange)
            .IsInEnum().When(x => x.BudgetRange.HasValue).WithMessage("Invalid budget range.");


        RuleFor(x => x.HourlyRateRange)
             .IsInEnum().When(x => x.HourlyRateRange.HasValue).WithMessage("Invalid hourly rate range.");

        RuleFor(x => x.ExperienceLevel)
            .IsInEnum().When(x => x.ExperienceLevel.HasValue).WithMessage("Invalid experience level.");



    }
}