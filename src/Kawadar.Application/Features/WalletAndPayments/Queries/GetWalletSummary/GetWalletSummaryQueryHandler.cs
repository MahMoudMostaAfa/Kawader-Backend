using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetWalletSummary;

public class GetWalletSummaryQueryHandler(
    IUser user,
    IUsersRepository usersRepository,
    IWalletRepository walletRepository,
    IMapper mapper) : IRequestHandler<GetWalletSummaryQuery, Result<PaginatedList<TransactionDto>>>
{
    public async Task<Result<PaginatedList<TransactionDto>>> Handle(GetWalletSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = user.Id;
        if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

        var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
        if (userProfileResult.IsError) return userProfileResult.Errors;
        var userProfile = userProfileResult.Value;

        var walletResult = await walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
        if (walletResult.IsError) return walletResult.Errors;

        var wallet = walletResult.Value;

        var transactions = await walletRepository.GetAllTransactionsByWalletId(
            wallet.Id,
            request.Type,
            request.Status,
            request.ReferenceType,
            request.Page,
            request.PageSize,
            request.SortBy,
            cancellationToken);

        var transactionDtos = transactions.Items.Select(t => mapper.Map<TransactionDto>(t)).ToList();

        return new PaginatedList<TransactionDto>(transactionDtos, transactions.TotalCount, request.Page, request.PageSize);
    }
}
