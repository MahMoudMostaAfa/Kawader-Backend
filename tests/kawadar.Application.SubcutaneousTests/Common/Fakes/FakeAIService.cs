using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeAIService : IAIService
{
    public Task<Result<T>> GenerateStructuredResponseAsync<T>(string prompt, CancellationToken ct = default) where T : class
    {
        var instance = Activator.CreateInstance<T>();
        return Task.FromResult<Result<T>>(instance);
    }

    public Task<Result<T>> GenerateStructuredResponseAsync<T>(string prompt, IEnumerable<FileData> images, CancellationToken ct = default) where T : class
    {
        var instance = Activator.CreateInstance<T>();
        return Task.FromResult<Result<T>>(instance);
    }
}
