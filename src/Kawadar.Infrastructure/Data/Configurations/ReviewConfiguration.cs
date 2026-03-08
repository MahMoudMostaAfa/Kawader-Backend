using Kawadar.Domain.Jobs;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReviewType).HasConversion<string>().IsRequired();

            builder.Property(x => x.Rating).IsRequired();

            builder.HasOne<Job>()
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.RevieweeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.ReviewerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
