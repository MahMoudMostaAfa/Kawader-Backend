
using FluentValidation;

namespace Kawadar.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.jobSlug).NotNull().WithMessage("Job Slug is required")
                .NotEmpty().WithMessage("Job Slug can't be empty");

            RuleFor(x => x.RevieweeUserName).NotNull().WithMessage("Reviewee UserName is required")
                .NotEmpty().WithMessage("Reviewee UserName can't be empty");

            RuleFor(x => x.rating).InclusiveBetween(0, 5).WithMessage("The rating must be between 0 and 5");

            RuleFor(x => x.comment).NotNull().WithMessage("Review comment is required")
                .NotEmpty().WithMessage("Review comment can't be empty");
        }
    }
}
