using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Queries.GetContractDetails;


public record GetContractDetailsQuery(Guid ContractId) : IRequest<Result<ContractDetailsDto>>;