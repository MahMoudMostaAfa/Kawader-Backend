namespace Kawadar.Application.Common.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kawadar.Domain.Common.Results;

public interface IEmailService
{
  Task<Result<Success>> SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
  Task<Result<Success>> SendManyAsync(IEnumerable<string> tos, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
