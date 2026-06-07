using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class ContractsRepository : IContractsRepository
{
  private readonly AppDbContext _context;

  public ContractsRepository(AppDbContext appDbContext)
  {
    _context = appDbContext;
  }
  public void Add(Contract contract)
  {
    _context.Contracts.Add(contract);
  }

  public async Task<Result<Contract>> GetContractByIdAsync(Guid contractId, CancellationToken cancellationToken)
  {
    var contract = await _context.Contracts.Include(c => c.ContractMilestones).FirstOrDefaultAsync(c => c.Id == contractId);
    if (contract is null) return Error.NotFound("Contracts.ContractNotFound", "Contract not found.");

    return contract;
  }

  public async Task<Result<PaginatedList<Contract>>> GetContractsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
  {
    var query = _context.Contracts.Where(c => c.ClientId == userId || c.FreelancerId == userId).Include(c => c.ContractMilestones)
   .OrderByDescending(c => c.CreatedAt).AsQueryable();

    var totalCount = await query.CountAsync();

    var contracts = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

    var paginatedList = new PaginatedList<Contract>(contracts, totalCount, pageNumber, pageSize);
    return paginatedList;
  }
}