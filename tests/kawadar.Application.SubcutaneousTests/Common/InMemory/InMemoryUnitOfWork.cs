using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common;
using MediatR;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly IMediator _mediator;
    private readonly InMemoryUsersRepository _usersRepository;
    private readonly InMemoryJobsRepository _jobsRepository;
    private readonly InMemoryProposalsRepository _proposalsRepository;

    public InMemoryUnitOfWork(
        IMediator mediator,
        InMemoryUsersRepository usersRepository,
        InMemoryJobsRepository jobsRepository,
        InMemoryProposalsRepository proposalsRepository)
    {
        _mediator = mediator;
        _usersRepository = usersRepository;
        _jobsRepository = jobsRepository;
        _proposalsRepository = proposalsRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect all entities with pending domain events across repositories
        var allEntities = new List<Entity>();
        allEntities.AddRange(_usersRepository.Users);
        allEntities.AddRange(_jobsRepository.Jobs);
        allEntities.AddRange(_proposalsRepository.Proposals);

        var domainEntities = allEntities
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events before publishing to prevent infinite loops
        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return 1;
    }
}
