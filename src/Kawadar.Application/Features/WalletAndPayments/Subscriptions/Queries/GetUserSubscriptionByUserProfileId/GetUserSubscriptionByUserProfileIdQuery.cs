using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetUserSubscriptionByUserProfileId
{
    public record GetUserSubscriptionByUserProfileIdQuery(UserSubscriptionStatus? status, int page, int pageSize, string sortBy) : IRequest<Result<PaginatedList<UserSubscriptionDto>>>;
}
