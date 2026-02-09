using Kawadar.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Domain.StorageRepository
{
    public interface IStorageClient
    {
        public Task<Result<String>> UploadFileAsync(Stream stream, string fileName, string container, CancellationToken cancellationToken);

        public Task<Result<Deleted>> DeleteFileAsync(string fileUrl, string container);
        public Task<Result<Success>> DownloadFileAsync(string blobName, string container,
            string filePath, CancellationToken cancellationToken);
    }
}
