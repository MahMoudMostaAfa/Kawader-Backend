
using Kawadar.Domain.Badges.FreelancerBadges;
using Xunit;

namespace kawadar.Domain.UnitTests.Badges
{
    public class FreelancerBadgeTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            Guid freelancerId = Guid.NewGuid();
            Guid badgeId = Guid.NewGuid();

            var result = FreelancerBadge.Create(freelancerId, badgeId);
            Assert.True(result.IsSuccess);
            var freelancerBadge = result.Value;
            Assert.Equal(freelancerId, freelancerBadge.FreelancerId);
            Assert.Equal(badgeId, freelancerBadge.BadgeId);
        }

        [Fact]
        public void Create_WithEmptyFreelancerId_ShouldSucceed()
        {
            Guid freelancerId = Guid.Empty;
            Guid badgeId = Guid.NewGuid();

            var result = FreelancerBadge.Create(freelancerId, badgeId);
            Assert.True(result.IsError);
            Assert.Equal(FreelancerBadgeErrors.FreelancerIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(FreelancerBadgeErrors.FreelancerIdIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyBadgeId_ShouldSucceed()
        {
            Guid freelancerId = Guid.NewGuid();
            Guid badgeId = Guid.Empty;

            var result = FreelancerBadge.Create(freelancerId, badgeId);
            Assert.True(result.IsError);
            Assert.Equal(FreelancerBadgeErrors.BadgeIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(FreelancerBadgeErrors.BadgeIdIsRequired.Description, result.TopError.Description);
        }
    }
}
