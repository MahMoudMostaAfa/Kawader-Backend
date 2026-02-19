using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IStorageClient
    {
        public Task<Result<String>> UploadFileAsync(Stream stream, string fileName, string container, CancellationToken cancellationToken);

        public Task<Result<Deleted>> DeleteFileAsync(string fileUrl, string container);
        public Task<Result<Success>> DownloadFileAsync(string blobName, string container,
            string filePath, CancellationToken cancellationToken);
    }
}
