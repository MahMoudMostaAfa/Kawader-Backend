using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Tests.Common.Jobs;
using Xunit;

namespace kawadar.Domain.UnitTests.Jobs
{
    public class JobTests
    {
        // Create
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = JobFactory.Builder().BuildResult();

            Assert.True(result.IsSuccess);
            Assert.Equal(JobStatus.Open, result.Value.JobStatus);
            Assert.Equal("Senior Backend Developer", result.Value.Title);
            Assert.Empty(result.Value.Attachments);
            Assert.Empty(result.Value.Questions);
            Assert.Empty(result.Value.Skills);
        }

        // Slug
        [Fact]
        public void GenerateSlug_WithValidTitle_ShouldReturnSlugWithSuffix()
        {
            var result = Job.GenerateSlug("Senior Backend Developer");

            Assert.True(result.IsSuccess);
            Assert.StartsWith("senior-backend-developer-", result.Value);
            Assert.Equal(33, result.Value.Length);
        }

        [Fact]
        public void GenerateSlug_WithSameTitleTwice_ShouldReturnDifferentValues()
        {
            var first = Job.GenerateSlug("Backend");
            var second = Job.GenerateSlug("Backend");

            Assert.True(first.IsSuccess);
            Assert.True(second.IsSuccess);
            Assert.NotEqual(first.Value, second.Value);
        }

        // Update
        [Fact]
        public void Update_WithNewTitle_ShouldUpdateTitleAndSlug()
        {
            var job = JobFactory.CreateValid();
            var originalSlug = job.JobSlug;

            var result = job.Update("Lead Backend Engineer", null, null, null, null, null, null, null);

            Assert.True(result.IsSuccess);
            Assert.Equal("Lead Backend Engineer", job.Title);
            Assert.NotEqual(originalSlug, job.JobSlug);
        }

        [Fact]
        public void Update_WithNullValues_ShouldKeepExistingValues()
        {
            var job = JobFactory.CreateValid();
            var originalTitle = job.Title;
            var originalDescription = job.Description;
            var originalDuration = job.DurationInDays;

            var result = job.Update(null, null, null, null, null, null, null, null);

            Assert.True(result.IsSuccess);
            Assert.Equal(originalTitle, job.Title);
            Assert.Equal(originalDescription, job.Description);
            Assert.Equal(originalDuration, job.DurationInDays);
        }

        [Fact]
        public void Update_WithNewSpecilizationId_ShouldUpdateSpecilizationId()
        {
            var job = JobFactory.CreateValid();
            var newSpecilizationId = Guid.NewGuid();

            var result = job.Update(null, null, null, null, null, null, null, newSpecilizationId);

            Assert.True(result.IsSuccess);
            Assert.Equal(newSpecilizationId, job.SpecilizationId);
        }

        // Attachments
        [Fact]
        public void AddAttachment_WhenCountIsLessThanFive_ShouldSucceed()
        {
            var job = JobFactory.CreateValid();

            var result = job.AddAttachment(JobFactory.CreateAttachment());

            Assert.True(result.IsSuccess);
            Assert.Single(job.Attachments);
        }

        [Fact]
        public void AddAttachment_WhenCountExceedsFive_ShouldFail()
        {
            var job = JobFactory.CreateValid();
            for (int i = 0; i < 5; i++)
            {
                job.AddAttachment(JobFactory.CreateAttachment(fileName: $"file-{i}.pdf"));
            }

            var result = job.AddAttachment(JobFactory.CreateAttachment(fileName: "file-6.pdf"));

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.MaxAttachmentsExceeded.Code, result.TopError.Code);
            Assert.Equal(JobErrors.MaxAttachmentsExceeded.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveAttachment_WithExistingAttachment_ShouldSucceed()
        {
            var job = JobFactory.CreateValid();
            var attachment = JobFactory.CreateAttachment();
            job.AddAttachment(attachment);

            var result = job.RemoveAttachment(attachment.Id);

            Assert.True(result.IsSuccess);
            Assert.Empty(job.Attachments);
        }

        [Fact]
        public void RemoveAttachment_WithUnknownAttachment_ShouldFail()
        {
            var job = JobFactory.CreateValid();

            var result = job.RemoveAttachment(Guid.NewGuid());

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.JobFileNotFound.Code, result.TopError.Code);
            Assert.Equal(JobErrors.JobFileNotFound.Description, result.TopError.Description);
        }

        // Questions
        [Fact]
        public void AddQuestion_WhenCountIsLessThanFive_ShouldSucceed()
        {
            var job = JobFactory.CreateValid();

            var result = job.AddQuestion(JobFactory.CreateQuestion());

            Assert.True(result.IsSuccess);
            Assert.Single(job.Questions);
        }

        [Fact]
        public void AddQuestion_WhenCountExceedsFive_ShouldFail()
        {
            var job = JobFactory.CreateValid();
            for (int i = 1; i <= 5; i++)
            {
                job.AddQuestion(JobFactory.CreateQuestion($"Question {i}", displayOrder: i));
            }

            var result = job.AddQuestion(JobFactory.CreateQuestion("Question 6", displayOrder: 6));

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.MaxQuestionsExceeded.Code, result.TopError.Code);
            Assert.Equal(JobErrors.MaxQuestionsExceeded.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveQuestion_WithExistingQuestion_ShouldSucceedAndReorderRemainingQuestions()
        {
            var job = JobFactory.CreateValid();
            var first = JobFactory.CreateQuestion("Q1", displayOrder: 1);
            var second = JobFactory.CreateQuestion("Q2", displayOrder: 2);
            var third = JobFactory.CreateQuestion("Q3", displayOrder: 3);
            job.AddQuestion(first);
            job.AddQuestion(second);
            job.AddQuestion(third);

            var result = job.RemoveQuestion(second.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, job.Questions.Count);
            Assert.Equal(1, job.Questions.Single(q => q.Id == first.Id).DisplayOrder);
            Assert.Equal(2, job.Questions.Single(q => q.Id == third.Id).DisplayOrder);
        }

        [Fact]
        public void RemoveQuestion_WithUnknownQuestion_ShouldFail()
        {
            var job = JobFactory.CreateValid();

            var result = job.RemoveQuestion(Guid.NewGuid());

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.JobQuestionNotFound.Code, result.TopError.Code);
            Assert.Equal(JobErrors.JobQuestionNotFound.Description, result.TopError.Description);
        }

        // Skills
        [Fact]
        public void AddSkill_WhenCountIsLessThanTen_ShouldSucceed()
        {
            var job = JobFactory.CreateValid();

            var result = job.AddSkill(JobFactory.CreateSkill());

            Assert.True(result.IsSuccess);
            Assert.Single(job.Skills);
        }

        [Fact]
        public void AddSkill_WithDuplicateSkillId_ShouldFail()
        {
            var job = JobFactory.CreateValid();
            var skill = JobFactory.CreateSkill();
            job.AddSkill(skill);

            var result = job.AddSkill(skill);

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.JobSkillAlreadyAdded.Code, result.TopError.Code);
            Assert.Equal(JobErrors.JobSkillAlreadyAdded.Description, result.TopError.Description);
        }

        [Fact]
        public void AddSkill_WhenCountExceedsTen_ShouldFail()
        {
            var seededSkills = Enumerable.Range(0, 10)
                .Select(i => JobFactory.CreateSkill($"Skill {i}"))
                .ToList();
            var job = JobFactory.Builder().WithSkills(seededSkills).Build();

            var result = job.AddSkill(JobFactory.CreateSkill("Skill 10"));

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.MaxSkillsExceeded.Code, result.TopError.Code);
            Assert.Equal(JobErrors.MaxSkillsExceeded.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveSkill_WithExistingSkill_ShouldSucceed()
        {
            var job = JobFactory.CreateValid();
            var skill = JobFactory.CreateSkill();
            job.AddSkill(skill);

            var result = job.RemoveSkill(skill.Id);

            Assert.True(result.IsSuccess);
            Assert.Empty(job.Skills);
        }

        [Fact]
        public void RemoveSkill_WithUnknownSkill_ShouldFail()
        {
            var job = JobFactory.CreateValid();

            var result = job.RemoveSkill(Guid.NewGuid());

            Assert.True(result.IsError);
            Assert.Equal(JobErrors.JobSkillNotFound.Code, result.TopError.Code);
            Assert.Equal(JobErrors.JobSkillNotFound.Description, result.TopError.Description);
        }
    }
}
