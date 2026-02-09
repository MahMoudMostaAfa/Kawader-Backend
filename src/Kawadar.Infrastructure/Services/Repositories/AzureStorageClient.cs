using Azure.Storage.Blobs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.StorageRepository;
using System.Threading;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class AzureStorageClient(BlobServiceClient _blobServiceClient) : IStorageClient
    {
        public async Task<Result<Deleted>> DeleteFileAsync(string fileUrl, string container)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var blobName = uri.Segments.Last();

                var containterClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containterClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();

                return Result.Deleted;
            }
            catch(Exception ex)
            {
                return Error.Failure();
            }
            
        }

        public async Task<Result<string>> UploadFileAsync(Stream stream, string fileName,
            string container, CancellationToken cancellationToken)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(stream, cancellationToken);
                return blobClient.Uri.ToString();
            }

            catch(Exception ex) 
            {
                return Error.Failure();
            }
        }

        public async Task<Result<Success>> DownloadFileAsync(string blobName, string container,
            string filePath, CancellationToken cancellationToken)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(blobName);

                

                await blobClient.DownloadToAsync(filePath, cancellationToken);

                return Result.Success;
            }
            catch(Exception ex)
            {
                return Error.Failure();
            }
        }
    }
}
