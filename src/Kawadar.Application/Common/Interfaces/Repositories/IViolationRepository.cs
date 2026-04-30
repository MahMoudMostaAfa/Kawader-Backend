
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations;
using Kawadar.Domain.Violations.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IViolationRepository
    {
        Task<Result<Success>> AddViolation(Violation violation);
        Task<PaginatedList<Violation>> GetAllViolation(ViolationStatus? status, ViolationType? type , int page, int pageSize, string sortBy);
        Task<Result<Violation>> GetViolationById(Guid Id);
    }
}