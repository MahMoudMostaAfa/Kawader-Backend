using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Infrastructure.Services.CloudServices
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
            catch (Exception ex)
            {
                return Error.Failure("Failed to delete file from Azure Storage", ex.Message);
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

            catch (Exception ex)
            {
                return Error.Failure("Failed to upload file to Azure Storage", ex.Message);
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
            catch (Exception ex)
            {
                return Error.Failure("Failed to download file from Azure Storage", ex.Message);
            }
        }

        public Result<string> GetSasUrl(string fileUrl, string container, TimeSpan? expiry = null)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var blobName = uri.Segments.Last();

                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(blobName);

                if (!blobClient.CanGenerateSasUri)
                    return Error.Failure("SasUrl.GenerationFailed",
                        "The storage client is not configured with credentials that support SAS generation.");

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = container,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.Add(expiry ?? TimeSpan.FromHours(1))
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri.ToString();
            }
            catch (Exception ex)
            {
                return Error.Failure("SasUrl.GenerationFailed", ex.Message);
            }
        }
    }
}
