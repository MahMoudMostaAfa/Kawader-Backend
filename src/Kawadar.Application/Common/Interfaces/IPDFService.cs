using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces;

public interface IPDFService
{
  Result<string> ExtractTextFromPdfAsync(Stream pdfStream, CancellationToken cancellationToken = default);
}