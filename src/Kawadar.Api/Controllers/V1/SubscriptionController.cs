using Kawadar.Api.Requests.Subscriptions;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CancelSubscription;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CreateSubscriptionPlan;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.DeleteSubscriptionPlanCommand;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.SubscribeToPlan;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.UpdateSubscriptionPlan;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetSubscriptionPlans;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetUserSubscriptionByUserProfileId;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Subscription")]
    public class SubscriptionController : ApiController
    {
        private readonly ISender _sender;
        public SubscriptionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<SubscriptionPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAllSubscriptions")]
        [EndpointSummary("Gets all subscriptions")]
        [EndpointDescription("Gets all subscriptions with the plan features.")]
        public async Task<IActionResult> GetAllSubscriptions(CancellationToken ct)
        {
            var query = new GetSubscriptionPlansQuery();
            var result = await _sender.Send(query, ct);

            return result.Match(
                subscriptions => Ok(subscriptions)
                , errors => Problem(errors));

        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Result<Created>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("AddSubscriptionPlan")]
        [EndpointSummary("Adds a SubscriptionPlan")]
        [EndpointDescription("Adds a subscription plan with the specified plan features.")]
        public async Task<IActionResult> AddSubscription(CreateSubscriptionPlanCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created()
                , errors => Problem(errors));
        }

        [HttpPut("{Id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateSubscriptionPlan")]
        [EndpointSummary("Updates a SubscriptionPlan")]
        [EndpointDescription("Upfates a subscription plan with the specified plan features.")]
        public async Task<IActionResult> UpdateSubscription([FromRoute] Guid Id, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken ct)
        {
            var command = new UpdateSubscriptionPlanCommand(Id, request.price, request.proposalsPerMonth, request.PortfolioProjects, request.twentyFourSevenSupport);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));
        }

        [HttpGet("User")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedList<UserSubscriptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAllUserSubscriptions")]
        [EndpointSummary("Gets all user subscriptions")]
        [EndpointDescription("Gets all current and previous user subscriptions.")]
        public async Task<IActionResult> GetAllUserSubscriptions(
            UserSubscriptionStatus? status,
            int page = 1,
            int pageSize = 10,
            string sortBy = "newest",
            CancellationToken ct = default)
        {
            var query = new GetUserSubscriptionByUserProfileIdQuery(status, page, pageSize, sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                userSubscriptions => Ok(userSubscriptions)
                , errors => Problem(errors));

        }

        [HttpPost("{Id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Created>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("SubscribeToPlan")]
        [EndpointSummary("Subscribe To a plan")]
        [EndpointDescription("subscribe to a plan with the specified features.")]
        public async Task<IActionResult> SubscribeToPlan([FromRoute] Guid Id, [FromBody] SubscribeToPlanRequest request, CancellationToken ct)
        {
            var command = new SubscribeToPlanCommand(Id, request.autoRenew);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created()
                , errors => Problem(errors));

        }

        [HttpDelete("{Id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteSubscribtionPlan")]
        [EndpointSummary("Deletes a subscription plan")]
        [EndpointDescription("Deletes a subscription plan with the specified Id.")]
        public async Task<IActionResult> DeleteSubscribtion([FromRoute] Guid Id, CancellationToken ct)
        {
            var command = new DeleteSubscriptionPlanCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));

        }

        [HttpPut("User/{Id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedList<UserSubscriptionDto>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CancelSubscription")]
        [EndpointSummary("Cancels user subscription")]
        [EndpointDescription("changes the status of a user subscription to canclled.")]
        public async Task<IActionResult> cancelSubscriptions([FromRoute] Guid Id, CancellationToken ct)
        {
            var command = new CancelSubscriptionCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));

        }
    }
}
