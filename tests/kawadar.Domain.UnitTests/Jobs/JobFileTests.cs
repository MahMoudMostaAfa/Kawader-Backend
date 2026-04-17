using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Tests.Common.Jobs;
using Xunit;

namespace kawadar.Domain.UnitTests.Jobs
{
    public class JobFileTests
    {
        [Fact]
        public void Create_WithValidFile_ShouldSucceed()
        {
            var fileInfo = JobFactory.CreateFileInfo();

            var result = JobFile.Create(fileInfo);

            Assert.True(result.IsSuccess);
            Assert.Equal(fileInfo.FileName, result.Value.File.FileName);
            Assert.Equal(fileInfo.FileUrl, result.Value.File.FileUrl);
        }

        [Fact]
        public void Create_WithImageFile_ShouldSucceed()
        {
            var fileInfo = JobFactory.CreateFileInfo("preview.png", "/uploads/jobs/preview.png", 2048, "image/png");

            var result = JobFile.Create(fileInfo);

            Assert.True(result.IsSuccess);
            Assert.Equal("image/png", result.Value.File.MimeType);
        }

        [Fact]
        public void Create_WithZeroSizeFile_ShouldSucceed()
        {
            var fileInfo = JobFactory.CreateFileInfo(fileSizeInBytes: 0);

            var result = JobFile.Create(fileInfo);

            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value.File.FileSizeInBytes);
        }

        [Fact]
        public void Create_WithEmptyMetadata_ShouldSucceed()
        {
            var fileInfo = JobFactory.CreateFileInfo(string.Empty, string.Empty, 0, string.Empty);

            var result = JobFile.Create(fileInfo);

            Assert.True(result.IsSuccess);
            Assert.Equal(string.Empty, result.Value.File.FileName);
            Assert.Equal(string.Empty, result.Value.File.FileUrl);
            Assert.Equal(string.Empty, result.Value.File.MimeType);
        }

        [Fact]
        public void Create_WithFileInfo_ShouldGenerateEntityId()
        {
            var result = JobFile.Create(JobFactory.CreateFileInfo());

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value.Id);
        }
    }
}
