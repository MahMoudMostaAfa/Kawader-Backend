using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbuteById
{
    public class GetDisbuteByIdQueryHandler(IUser user, IDisbuteRepository disbuteRepository,
        IMapper mapper, IIdentityService identityService, IUsersRepository usersRepository, IContractsRepository contractsRepository) : IRequestHandler<GetDisbuteByIdQuery, Result<fullDisbuteDto>>
    {
        public async Task<Result<fullDisbuteDto>> Handle(GetDisbuteByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var disbuteResult = await disbuteRepository.GetDisbuteById(request.Id);
            if (disbuteResult.IsError) return disbuteResult.Errors;

            var contractResult = await contractsRepository.GetContractByIdAsync(disbuteResult.Value.ContractId);
            if (contractResult.IsError) return contractResult.Errors;

            var clientProfile = await usersRepository.GetUserProfileByIdAsync(contractResult.Value.ClientId);
            if (clientProfile.IsError) return clientProfile.Errors;

            var clientDto = await identityService.GetUserByIdAsync(clientProfile.Value.UserId);
            if (clientDto.IsError) return clientDto.Errors;

            var freelancerProfile = await usersRepository.GetUserProfileByIdAsync(contractResult.Value.FreelancerId);
            if (freelancerProfile.IsError) return freelancerProfile.Errors;

            var freelancerDto = await identityService.GetUserByIdAsync(freelancerProfile.Value.UserId);
            if (freelancerDto.IsError) return freelancerDto.Errors;

            if (clientProfile.Value.Id == disbuteResult.Value.RaisedById)
            {
                var disbuteDto = mapper.Map<fullDisbuteDto>((disbuteResult.Value, clientDto.Value));
                var adminContractDto = mapper.Map<AdminContractDto>((contractResult.Value, freelancerDto.Value, clientDto.Value));
                disbuteDto.contract = adminContractDto;
                return disbuteDto;
            }

            else if(freelancerProfile.Value.Id == disbuteResult.Value.RaisedById)
            {
                var disbuteDto = mapper.Map<fullDisbuteDto>((disbuteResult.Value, freelancerDto.Value));
                var adminContractDto = mapper.Map<AdminContractDto>((contractResult.Value, freelancerDto.Value, clientDto.Value));
                disbuteDto.contract = adminContractDto;
                return disbuteDto;
            }
            else
            {
                return Error.Validation("The Disbute was not Raised by a contributor in the contract");
            }
        }
    }
}
