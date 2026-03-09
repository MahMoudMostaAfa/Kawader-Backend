using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(string jobSlug, string RevieweeUserName, float rating, string comment) : IRequest<Result<Success>>;
}
