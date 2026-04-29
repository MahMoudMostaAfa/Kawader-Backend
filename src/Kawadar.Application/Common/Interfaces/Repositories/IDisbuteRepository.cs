using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes;
using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IDisbuteRepository
    {
        Task<Result<Success>> AddDisbute(Disbute disbute);
        Task<Result<Disbute>> GetDisbuteById(Guid Id);
        Task<PaginatedList<Disbute>> GetAllDisbutes(DisbuteStatus? status, int page, int pageSize, string sortBy);
    }
}
