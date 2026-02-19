using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles;
using Kawadar.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(up => up.Id);
        builder.HasIndex(up => up.UserId).IsUnique();

        builder.Property(up => up.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(up => up.LastName).IsRequired().HasMaxLength(50);

        builder.Property(up => up.Bio)
            .HasMaxLength(1000);



        builder.Property(up => up.Title)
                .HasMaxLength(100);


        builder.Property(up => up.ExperienceYear)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(up => up.ProfileType).HasConversion<string>()
            .IsRequired();


        builder.Property(up => up.IsAvailable)
            .IsRequired();

        builder.Property(up => up.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(up => up.VideoLink)
            .HasMaxLength(200);


        builder.Property(up => up.CreatedAt)
            .IsRequired();

        builder.Property(up => up.UpdatedAt)
            .IsRequired();



        builder.Property(up => up.IdentityImgUrl)
            .HasMaxLength(200);

        builder.Property(up => up.IdentityImgBackUrl)
              .HasMaxLength(200);
        builder.Property(up => up.IdentityLocation).HasMaxLength(200);
        builder.Property(up => up.IdentityName).HasMaxLength(200);
        builder.Property(up => up.IdentityNumber).HasMaxLength(20);
        builder.Property(up => up.DateOfBirth).HasColumnType("date");

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Specialization)
            .WithMany()
            .HasForeignKey(up => up.SpecializationId)
            .OnDelete(DeleteBehavior.SetNull);


        builder.Ignore(up => up.FullName);
    }
}
