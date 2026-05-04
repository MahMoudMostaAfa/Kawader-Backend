using Kawadar.Application.Common.Messaging;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

/// <summary>
/// Captures both Publish and Send calls so tests can assert on specific message
/// payloads, types, and queue names independently.
/// </summary>
public class FakeEventBus : IEventBus
{
    // ── Published messages (fan-out / topic-based) ───────────────────────────
    public readonly List<object>  PublishedMessages = [];
    public readonly List<Type>    PublishedMessageTypes = [];

    // ── Sent messages (point-to-point / queue-based) ─────────────────────────
    public readonly List<object>  SentMessages = [];
    public readonly List<Type>    SentMessageTypes = [];
    public readonly List<string>  SentQueueNames = [];

    // ── Convenience: everything in arrival order ──────────────────────────────
    public IReadOnlyList<object> AllMessages =>
        PublishedMessages.Concat(SentMessages).ToList();

    // ── IEventBus ─────────────────────────────────────────────────────────────

    public Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        PublishedMessages.Add(message);
        PublishedMessageTypes.Add(typeof(T));
        return Task.CompletedTask;
    }

    public Task SendAsync<T>(T message, string queueName, CancellationToken ct = default) where T : class
    {
        SentMessages.Add(message);
        SentMessageTypes.Add(typeof(T));
        SentQueueNames.Add(queueName);
        return Task.CompletedTask;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Returns all published messages of the given type.</summary>
    public IEnumerable<T> PublishedOf<T>() where T : class
        => PublishedMessages.OfType<T>();

    /// <summary>Returns all sent messages of the given type.</summary>
    public IEnumerable<T> SentOf<T>() where T : class
        => SentMessages.OfType<T>();

    public void Clear()
    {
        PublishedMessages.Clear();
        PublishedMessageTypes.Clear();
        SentMessages.Clear();
        SentMessageTypes.Clear();
        SentQueueNames.Clear();
    }
}
