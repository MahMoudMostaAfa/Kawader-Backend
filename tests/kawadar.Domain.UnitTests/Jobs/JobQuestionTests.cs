using Kawadar.Domain.Jobs.JobQuestions;
using Xunit;

namespace kawadar.Domain.UnitTests.Jobs
{
    public class JobQuestionTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = JobQuestion.Create("Do you have similar experience?", true, 1);

            Assert.True(result.IsSuccess);
            Assert.Equal("Do you have similar experience?", result.Value.Question);
            Assert.True(result.Value.IsRequired);
            Assert.Equal(1, result.Value.DisplayOrder);
        }

        [Fact]
        public void CreateList_WithMultipleQuestions_ShouldAssignSequentialDisplayOrder()
        {
            var inputs = new List<(string question, bool isRequired)>
            {
                ("Q1", true),
                ("Q2", false),
                ("Q3", true),
            };

            var result = JobQuestion.CreateList(inputs);

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal(1, result.Value[0].DisplayOrder);
            Assert.Equal(2, result.Value[1].DisplayOrder);
            Assert.Equal(3, result.Value[2].DisplayOrder);
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateQuestionProperties()
        {
            var question = JobQuestion.Create("Old", false, 1).Value;

            question.Update("New", true, 4);

            Assert.Equal("New", question.Question);
            Assert.True(question.IsRequired);
            Assert.Equal(4, question.DisplayOrder);
        }
    }
}
