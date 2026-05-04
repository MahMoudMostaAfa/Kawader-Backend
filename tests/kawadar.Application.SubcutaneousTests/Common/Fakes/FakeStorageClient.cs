using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeStorageClient : IStorageClient
{
    public Task<Result<string>> UploadFileAsync(Stream stream, string fileName, string container, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Result<string>>($"https://fake-storage/{container}/{fileName}");
    }

    public Task<Result<Deleted>> DeleteFileAsync(string fileUrl, string container)
    {
        return Task.FromResult<Result<Deleted>>(Result.Deleted);
    }

    public Task<Result<Success>> DownloadFileAsync(string blobName, string container, string filePath, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Result<string> GetSasUrl(string fileUrl, string container, TimeSpan? expiry = null)
    {
        return fileUrl;
    }
}
