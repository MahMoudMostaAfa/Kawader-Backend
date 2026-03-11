using Kawadar.Domain.Proposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ProposalsConfigurations;

public class ProposalConfiguration : IEntityTypeConfiguration<JobProposal>
{
       public void Configure(EntityTypeBuilder<JobProposal> builder)
       {
              builder.HasKey(p => p.Id);

              builder.Property(p => p.CoverLetter)
                     .IsRequired()
                     .HasMaxLength(200);

              builder.Property(p => p.ProposalType).HasConversion<string>()
                     .IsRequired();

              builder.Property(p => p.Amount)
                       .HasPrecision(18, 2);



              builder.Property(p => p.EstimatedDays);

              builder.Property(p => p.Status).HasConversion<string>()
                     .IsRequired();

              builder.HasIndex(p => new { p.JobId, p.FreelancerId }).IsUnique();



              builder.HasMany(p => p.Milestones)
                     .WithOne()
                     .HasForeignKey(m => m.JobProposalId)
                     .OnDelete(DeleteBehavior.Cascade);
              builder.Navigation(p => p.Milestones).HasField("_milestones").UsePropertyAccessMode(PropertyAccessMode.Field);


              builder.HasMany(p => p.QuestionAnswers)
                     .WithOne()
                     .HasForeignKey(qa => qa.JobProposalId)
                     .OnDelete(DeleteBehavior.Cascade);
              builder.Navigation(p => p.QuestionAnswers).HasField("_questionAnswers").UsePropertyAccessMode(PropertyAccessMode.Field);


       }
}