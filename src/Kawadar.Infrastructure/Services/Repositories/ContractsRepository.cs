using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Contracts;
using Kawadar.Infrastructure.Data;

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
}