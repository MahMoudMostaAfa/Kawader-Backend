using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Queries.GetMyContracts;

public record GetMyContractsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedList<ContractDto>>>;