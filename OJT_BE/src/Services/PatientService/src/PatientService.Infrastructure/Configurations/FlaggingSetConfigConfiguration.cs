using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Configurations
{
    public class FlaggingSetConfigConfiguration : IEntityTypeConfiguration<FlaggingSetConfig>
    {
        public void Configure(EntityTypeBuilder<FlaggingSetConfig> builder)
        {
            builder.ToTable("FlaggingSetConfig");

            builder.HasKey(x => x.ConfigId);

            builder.Property(x => x.TestName)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(x => x.Version)
                   .HasColumnType("text")
                   .HasDefaultValue("1.0");
        }
    }
}
