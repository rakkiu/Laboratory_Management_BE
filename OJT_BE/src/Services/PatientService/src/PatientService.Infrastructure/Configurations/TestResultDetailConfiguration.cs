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
    public class TestResultDetailConfiguration : IEntityTypeConfiguration<TestResultDetail>
    {
        public void Configure(EntityTypeBuilder<TestResultDetail> builder)
        {
            builder.ToTable("TestResultDetail");

            builder.HasKey(x => x.TestResultDetailId);

            builder.Property(x => x.Type)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(x => x.Value)
                   .IsRequired();

            builder.Property(x => x.Flag)
                   .HasColumnType("text")
                   .IsRequired()
                   .HasDefaultValue("Normal");

            builder.Property(x => x.ReferenceRange)
                   .HasColumnType("text");

            builder.HasOne(x => x.TestResult)
                   .WithMany(r => r.TestResultDetails)
                   .HasForeignKey(x => x.ResultId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
