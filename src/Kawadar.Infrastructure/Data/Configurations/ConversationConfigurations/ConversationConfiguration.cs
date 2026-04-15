using Kawadar.Domain.Conversations;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ConversationConfigurations;


public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
  public void Configure(EntityTypeBuilder<Conversation> builder)
  {
    builder.HasKey(mf => mf.Id);

    builder.Property(c => c.ConversationStatus).HasConversion<string>().IsRequired();

    builder.HasOne(c => c.LastMessage)
        .WithMany()
        .HasForeignKey(c => c.LastMessageId)
        .OnDelete(DeleteBehavior.NoAction);

    builder.HasOne(c => c.Job)
        .WithMany()
        .HasForeignKey(c => c.JobId)
        .OnDelete(DeleteBehavior.SetNull);

    builder.HasMany(c => c.Messages)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<UserProfile>().WithMany().HasForeignKey(c => c.SenderUserId).OnDelete(DeleteBehavior.NoAction);
    builder.HasOne<UserProfile>().WithMany().HasForeignKey(c => c.ReceiverUserId).OnDelete(DeleteBehavior.NoAction);
  }
}