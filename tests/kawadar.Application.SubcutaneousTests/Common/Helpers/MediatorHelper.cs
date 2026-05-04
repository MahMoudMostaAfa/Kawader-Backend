using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace kawadar.Application.SubcutaneousTests.Common.Helpers;

public static class MediatorHelper
{
    public static Task<TResponse> Send<TResponse>(
        this AsyncServiceScope scope,
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return mediator.Send(request, ct);
    }
}
