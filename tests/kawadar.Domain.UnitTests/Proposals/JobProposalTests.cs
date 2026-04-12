using Kawadar.Domain.Proposals;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Tests.Common.Proposals;
using Xunit;

namespace kawadar.Domain.UnitTests.Proposals
{
    public class JobProposalTests
    {
        // Create
        [Fact]
        public void Create_WithValidOneTimeData_ShouldSucceed()
        {
            var result = JobProposalFactory.Builder().WithProposalType(JobProposalType.OneTime).BuildResult();

            Assert.True(result.IsSuccess);
            Assert.Equal(JobProposalStatus.Pending, result.Value.Status);
        }

        [Fact]
        public void Create_WithValidHourlyData_ShouldSucceed()
        {
            var builder = JobProposalFactory.Builder().WithProposalType(JobProposalType.Hourly)
                .WithoutAmount().WithHourlyRate(25).WithEstimatedHours(40).WithEstimatedDays(null);

            var result = builder.BuildResult();

            Assert.True(result.IsSuccess);
            Assert.Equal(JobProposalType.Hourly, result.Value.ProposalType);
        }

        [Fact]
        public void Create_WithEmptyCoverLetter_ShouldFail()
        {
            var result = JobProposalFactory.Builder().WithoutCoverLetter().BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.CoverLetterRequired.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.CoverLetterRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyJobId_ShouldFail()
        {
            var result = JobProposalFactory.Builder().WithoutJobId().BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.JobIDRequired.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.JobIDRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyFreelancerId_ShouldFail()
        {
            var result = JobProposalFactory.Builder().WithoutFreelancerId().BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.FreelancerIDRequired.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.FreelancerIDRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_OneTimeWithoutAmount_ShouldFail()
        {
            var result = JobProposalFactory.Builder().WithProposalType(JobProposalType.OneTime).WithoutAmount().BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.AmountRequiredForOneTime.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.AmountRequiredForOneTime.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_HourlyWithoutEstimatedHours_ShouldFail()
        {
            var builder = JobProposalFactory.Builder().WithProposalType(JobProposalType.Hourly)
                .WithoutAmount().WithHourlyRate(20).WithoutEstimatedHours();

            var result = builder.BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.EstimatedHoursRequiredForHourly.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.EstimatedHoursRequiredForHourly.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_HourlyWithoutHourlyRate_ShouldFail()
        {
            var builder = JobProposalFactory.Builder().WithProposalType(JobProposalType.Hourly)
                .WithoutAmount().WithoutHourlyRate().WithEstimatedHours(15);

            var result = builder.BuildResult();

            Assert.True(result.IsError);
            Assert.Equal(JobProposalErrors.AmountRequiredForOneTime.Code, result.TopError.Code);
            Assert.Equal(JobProposalErrors.AmountRequiredForOneTime.Description, result.TopError.Description);
        }

        // Update
        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();

            var result = proposal.Update("new cover", JobProposalType.Hourly, 450m, 30, 20, 10);

            Assert.True(result.IsSuccess);
            Assert.Equal(JobProposalType.Hourly, proposal.ProposalType);
            Assert.Equal(30, proposal.HourlyRate);
        }

        [Fact]
        public void Update_WithInvalidValues_ShouldNotOverrideExistingValues()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();

            var result = proposal.Update(string.Empty, null, 0, -1, 0, 0);

            Assert.True(result.IsSuccess);
            Assert.Equal("Valid cover letter", proposal.CoverLetter);
            Assert.Equal(600m, proposal.Amount);
        }

        // Milestones
        [Fact]
        public void AddMilestone_ForMilestoneBasedProposal_ShouldSucceed()
        {
            var proposal = JobProposalFactory.CreateValidMilestoneBased();
            var milestone = ProposalMilestoneFactory.CreateValid();

            var result = proposal.AddMilestone(milestone);

            Assert.True(result.IsSuccess);
            Assert.Single(proposal.Milestones);
        }

        [Fact]
        public void AddMilestone_ForNonMilestoneBasedProposal_ShouldFail()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();
            var expectedError = Error.Validation("Milestones can only be added to milestone-based proposals.");

            var result = proposal.AddMilestone(ProposalMilestoneFactory.CreateValid());

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveMilestone_WithExistingMilestone_ShouldSucceedAndReorder()
        {
            var proposal = JobProposalFactory.CreateValidMilestoneBased();
            var first = ProposalMilestoneFactory.Builder().WithDisplayOrder(1).Build();
            var second = ProposalMilestoneFactory.Builder().WithDisplayOrder(2).Build();
            var third = ProposalMilestoneFactory.Builder().WithDisplayOrder(3).Build();
            proposal.AddMilestone(first); proposal.AddMilestone(second); proposal.AddMilestone(third);

            var result = proposal.RemoveMilestone(second);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, third.DisplayOrder);
        }

        [Fact]
        public void RemoveMilestone_ForNonMilestoneBasedProposal_ShouldFail()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();
            var expectedError = Error.Validation("Milestones can only be removed from milestone-based proposals.");

            var result = proposal.RemoveMilestone(ProposalMilestoneFactory.CreateValid());

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveMilestone_WithUnknownMilestone_ShouldFail()
        {
            var proposal = JobProposalFactory.CreateValidMilestoneBased();
            var expectedError = Error.NotFound("Milestone not found.");

            var result = proposal.RemoveMilestone(ProposalMilestoneFactory.CreateValid());

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        // QuestionAnswers
        [Fact]
        public void AddQuestionAnswer_ShouldSucceed()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();

            var result = proposal.AddQuestionAnswer(ProposalQuestionAnswerFactory.CreateValid());

            Assert.True(result.IsSuccess);
            Assert.Single(proposal.QuestionAnswers);
        }

        [Fact]
        public void RemoveQuestionAnswer_WithUnknownAnswer_ShouldFail()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();
            var expectedError = Error.NotFound("Question answer not found.");

            var result = proposal.RemoveQuestionAnswer(ProposalQuestionAnswerFactory.CreateValid());

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void RemoveQuestionAnswer_WithExistingAnswer_ShouldSucceed()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();
            var questionAnswer = ProposalQuestionAnswerFactory.CreateValid();
            proposal.AddQuestionAnswer(questionAnswer);

            var result = proposal.RemoveQuestionAnswer(questionAnswer);

            Assert.True(result.IsSuccess);
            Assert.Empty(proposal.QuestionAnswers);
        }

        // State
        [Fact]
        public void UpdateState_WithNewState_ShouldSucceed()
        {
            var proposal = JobProposalFactory.CreateValidOneTime();

            var result = proposal.UpdateState(JobProposalStatus.Accepted);

            Assert.True(result.IsSuccess);
            Assert.Equal(JobProposalStatus.Accepted, proposal.Status);
        }
    }
}
