using kawadar.Application.SubcutaneousTests.Common.Fakes;
using kawadar.Application.SubcutaneousTests.Common.InMemory;
using Kawadar.Application;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

namespace kawadar.Application.SubcutaneousTests.Common.Fixtures;

public class SubcutaneousTestFixture
{
    public IServiceProvider Services { get; }

    public SubcutaneousTestFixture()
    {
        var services = new ServiceCollection();
        services.AddApplication(); // real MediatR + validators + behaviours
        services.AddHybridCache(); // required by CachingBehaviour pipeline
        services.AddLogging(b => b.AddProvider(Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider.Instance));

        // Config required by event handlers
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BackUrl"] = "https://test.local"
            })
            .Build());

        // In-memory repositories (Singleton so data persists within a test class)
        services.AddSingleton<InMemoryUsersRepository>();
        services.AddSingleton<IUsersRepository>(sp => sp.GetRequiredService<InMemoryUsersRepository>());
        services.AddSingleton<InMemoryJobsRepository>();
        services.AddSingleton<IJobsRepository>(sp => sp.GetRequiredService<InMemoryJobsRepository>());
        services.AddSingleton<InMemoryProposalsRepository>();
        services.AddSingleton<IProposalsRepository>(sp => sp.GetRequiredService<InMemoryProposalsRepository>());
        services.AddSingleton<InMemorySpecilizationRepository>();
        services.AddSingleton<ISpecilizationRepository>(sp => sp.GetRequiredService<InMemorySpecilizationRepository>());
        services.AddSingleton<InMemorySkillRepository>();
        services.AddSingleton<ISkillRepository>(sp => sp.GetRequiredService<InMemorySkillRepository>());
        services.AddSingleton<InMemoryReviewRepository>();
        services.AddSingleton<IReviewRepository>(sp => sp.GetRequiredService<InMemoryReviewRepository>());
        services.AddSingleton<InMemoryJobViewRepository>();
        services.AddSingleton<IJobViewRepository>(sp => sp.GetRequiredService<InMemoryJobViewRepository>());

        // Scoped so each test scope gets its own FakeUser
        services.AddScoped<FakeUser>();
        services.AddScoped<IUser>(sp => sp.GetRequiredService<FakeUser>());

        // Singleton fakes (no external state)
        services.AddSingleton<FakeIdentityService>();
        services.AddSingleton<IIdentityService>(sp => sp.GetRequiredService<FakeIdentityService>());
        services.AddSingleton<FakeStorageClient>();
        services.AddSingleton<IStorageClient>(sp => sp.GetRequiredService<FakeStorageClient>());
        services.AddSingleton<FakeEmailService>();
        services.AddSingleton<IEmailService>(sp => sp.GetRequiredService<FakeEmailService>());
        services.AddSingleton<FakeEmailTemplateService>();
        services.AddSingleton<IEmailTemplateService>(sp => sp.GetRequiredService<FakeEmailTemplateService>());
        services.AddSingleton<FakeAIService>();
        services.AddSingleton<IAIService>(sp => sp.GetRequiredService<FakeAIService>());
        services.AddSingleton<FakeEventBus>();
        services.AddSingleton<Kawadar.Application.Common.Messaging.IEventBus>(sp => sp.GetRequiredService<FakeEventBus>());
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        // Additional repositories required by newer handlers
        services.AddSingleton<InMemoryWalletRepository>();
        services.AddSingleton<IWalletRepository>(sp => sp.GetRequiredService<InMemoryWalletRepository>());
        services.AddSingleton<InMemorySubscriptionsRepository>();
        services.AddSingleton<ISubscriptionsRepository>(sp => sp.GetRequiredService<InMemorySubscriptionsRepository>());
        services.AddSingleton<InMemoryNotificationsRepository>();
        services.AddSingleton<Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers.INotificationsRepository>(sp => sp.GetRequiredService<InMemoryNotificationsRepository>());

        // Fakes for external AI/recommendation services
        services.AddSingleton<FakeFreelancerVectorStore>();
        services.AddSingleton<IFreelancerVectorStore>(sp => sp.GetRequiredService<FakeFreelancerVectorStore>());
        services.AddSingleton<FakeRecommendationService>();
        services.AddSingleton<IRecommendationService>(sp => sp.GetRequiredService<FakeRecommendationService>());
        services.AddSingleton<FakeNotificationsHubService>();
        services.AddSingleton<Kawadar.Application.Common.Hubs.INotificationsHubService>(sp => sp.GetRequiredService<FakeNotificationsHubService>());

        Services = services.BuildServiceProvider();
    }
}
