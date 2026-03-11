using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewsByUserProfileId
{
    public record GetReviewsByUserNameQuery(float? rating, int page, int pageSize, string sortBy, string userName) : IRequest<Result<PaginatedList<ReviewDto>>>;
}
