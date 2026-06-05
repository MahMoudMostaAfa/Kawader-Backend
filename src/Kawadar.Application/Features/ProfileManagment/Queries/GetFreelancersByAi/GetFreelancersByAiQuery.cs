using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancersByAi;


public record GetFreelancersByAiQuery(string Query) : IRequest<Result<IEnumerable<BriefFreelancerWithStrengthDto>>>;