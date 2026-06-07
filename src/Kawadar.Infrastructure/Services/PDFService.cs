using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Infrastructure.Services;

public class PDFService : IPDFService
{
  public Result<string> ExtractTextFromPdfAsync(Stream pdfStream, CancellationToken cancellationToken = default)
  {
    var textBuilder = new StringBuilder();

    using (var reader = new PdfReader(pdfStream))
    using (var pdfDoc = new PdfDocument(reader))
    {
      int pageCount = pdfDoc.GetNumberOfPages();

      for (int i = 1; i <= pageCount; i++)
      {
        var page = pdfDoc.GetPage(i);

        string pageText = PdfTextExtractor.GetTextFromPage(page);
        textBuilder.AppendLine(pageText);
      }
    }

    return textBuilder.ToString();
  }

}
