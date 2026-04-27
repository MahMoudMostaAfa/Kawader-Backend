using Kawadar.Domain.Contracts;
using Kawadar.Domain.Contracts.Disbutes;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ContractsConfigurations
{
    public class DisbuteConfiguration : IEntityTypeConfiguration<Disbute>
    {
        public void Configure(EntityTypeBuilder<Disbute> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Status).HasConversion<string>();
            builder.Property(x => x.Resolution).HasMaxLength(500);

            builder.HasOne<Contract>()
                .WithOne()
                .HasForeignKey<Disbute>(x => x.ContractId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.RaisedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
