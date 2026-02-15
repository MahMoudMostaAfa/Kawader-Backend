using Kawadar.Domain.Specilizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class SpecilizationConfiguration : IEntityTypeConfiguration<Specilization>
    {
        public void Configure(EntityTypeBuilder<Specilization> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).HasMaxLength(50).IsRequired();

            builder.Property(s => s.IsActive).IsRequired();

            builder.Property(s => s.CreatedAt).IsRequired();

            builder.Property(s => s.UpdatedAt).IsRequired();
        }
    }
}
