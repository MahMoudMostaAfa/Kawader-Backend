using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using System.Runtime.CompilerServices;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

/// <summary>
/// In-memory AI service that returns a default instance of the requested type.
/// Uses <see cref="FormatterServices.GetUninitializedObject"/> as a fallback so
/// that types without a parameterless constructor (e.g. records, sealed classes)
/// can still be instantiated without throwing.
/// </summary>
public class FakeAIService : IAIService
{
    /// <summary>
    /// Optional per-type factory overrides set by tests (keyed on <see cref="Type"/>).
    /// If no factory is registered the default empty instance is returned.
    /// </summary>
    private readonly Dictionary<Type, Func<object>> _factories = new();

    /// <summary>Register a factory so tests can control the returned value.</summary>
    public void Register<T>(Func<T> factory) where T : class
        => _factories[typeof(T)] = () => factory();

    public Task<Result<T>> GenerateStructuredResponseAsync<T>(
        string prompt,
        CancellationToken ct = default) where T : class
    {
        var instance = CreateInstance<T>();
        return Task.FromResult<Result<T>>(instance);
    }

    public Task<Result<T>> GenerateStructuredResponseAsync<T>(
        string prompt,
        IEnumerable<FileData> images,
        CancellationToken ct = default) where T : class
    {
        var instance = CreateInstance<T>();
        return Task.FromResult<Result<T>>(instance);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private T CreateInstance<T>() where T : class
    {
        if (_factories.TryGetValue(typeof(T), out var factory))
            return (T)factory();

        // Try parameterless constructor first; fall back to uninitialized object.
        try
        {
            return Activator.CreateInstance<T>();
        }
        catch (MissingMethodException)
        {
            return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        }
    }
}
