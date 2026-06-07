using Kawadar.Domain.Notifications;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.NotificationConfigurations;


public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
  public void Configure(EntityTypeBuilder<Notification> builder)
  {

    builder.HasKey(n => n.Id);
    builder.Property(n => n.Title).HasMaxLength(255).IsRequired();
    builder.Property(n => n.Body).HasMaxLength(2000).IsRequired();
    builder.HasOne<UserProfile>().WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
    builder.Property(n => n.Category).HasConversion<string>().HasMaxLength(50).IsRequired();
    builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
    builder.Property(n => n.ReferenceType).HasMaxLength(100);
    builder.Property(n => n.RedirectUrl).HasMaxLength(500);
    builder.HasIndex(n => n.UserId).IsUnique(false);


    builder.Property(n => n.IsRead).HasDefaultValue(false);

    builder.Property(n => n.ReadAt).HasDefaultValue(null);




  }
}