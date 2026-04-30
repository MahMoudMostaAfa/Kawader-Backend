using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public record SentEmail(string To, string Subject, string Body);

public class FakeEmailService : IEmailService
{
    public List<SentEmail> SentEmails { get; } = [];

    public Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        SentEmails.Add(new SentEmail(to, subject, htmlBody));
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> SendManyAsync(IEnumerable<string> tos, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        foreach (var to in tos)
        {
            SentEmails.Add(new SentEmail(to, subject, htmlBody));
        }
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public void Clear() => SentEmails.Clear();
}
