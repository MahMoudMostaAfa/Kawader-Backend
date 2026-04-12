using Kawadar.Domain.Proposals.QuestionAnswers;
using Kawadar.Domain.Common.Results;
using Kawadar.Tests.Common.Proposals;
using Xunit;

namespace kawadar.Domain.UnitTests.Proposals
{
    public class ProposalQuestionAnswerTests
    {
        // Create
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = ProposalQuestionAnswerFactory.Builder().BuildResult();

            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.Answer));
        }

        [Fact]
        public void Create_WithEmptyJobProposalId_ShouldFail()
        {
            var result = ProposalQuestionAnswerFactory.Builder().WithJobProposalId(Guid.Empty).BuildResult();
            var expectedError = Error.Validation("Job Proposal ID is required.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyQuestionId_ShouldFail()
        {
            var result = ProposalQuestionAnswerFactory.Builder().WithQuestionId(Guid.Empty).BuildResult();
            var expectedError = Error.Validation("Question ID is required.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyAnswer_ShouldFail()
        {
            var result = ProposalQuestionAnswerFactory.Builder().WithAnswer(string.Empty).BuildResult();
            var expectedError = Error.Validation("Answer is required.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        // Update
        [Fact]
        public void Update_WithValidAnswer_ShouldSucceed()
        {
            var questionAnswer = ProposalQuestionAnswerFactory.CreateValid();

            var result = questionAnswer.Update("new answer");

            Assert.True(result.IsSuccess);
            Assert.Equal("new answer", questionAnswer.Answer);
        }

        [Fact]
        public void Update_WithEmptyAnswer_ShouldFailAndKeepOriginalAnswer()
        {
            var questionAnswer = ProposalQuestionAnswerFactory.CreateValid();
            var originalAnswer = questionAnswer.Answer;
            var expectedError = Error.Validation("Answer is required.");

            var result = questionAnswer.Update(string.Empty);

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
            Assert.Equal(originalAnswer, questionAnswer.Answer);
        }
    }
}
