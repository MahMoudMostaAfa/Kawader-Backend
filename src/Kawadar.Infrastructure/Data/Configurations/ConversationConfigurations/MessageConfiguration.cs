using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ConversationConfigurations;


public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
  public void Configure(EntityTypeBuilder<Message> builder)
  {

    builder.HasKey(m => m.Id);
    builder.Property(m => m.Content).HasMaxLength(2000).IsRequired();

    builder.HasMany(m => m.Files)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);


    // builder.HasOne<Conversation>().WithMany().HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade)
    // ;
    builder.HasOne<UserProfile>().WithMany().HasForeignKey(m => m.SenderUserId).OnDelete(DeleteBehavior.Cascade);

    builder.Property(m => m.IsDeleted).HasDefaultValue(false);

    builder.HasOne(m => m.ReplayToMessage)
        .WithMany()
        .HasForeignKey(m => m.ReplayToMessageId)
        .OnDelete(DeleteBehavior.NoAction);


    builder.Navigation(m => m.Files)
        .HasField("_files")
        .UsePropertyAccessMode(PropertyAccessMode.Field);

  }
}