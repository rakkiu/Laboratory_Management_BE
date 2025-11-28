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
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.ToTable("TestResult");

            builder.HasKey(x => x.ResultId);

            builder.Property(x => x.TestName)
                   .HasColumnType("text")
                   .IsRequired();
            builder.Property(x => x.Interpretation)
                   .HasColumnType("text");

            builder.Property(x => x.InstrumentUsed)
                   .HasColumnType("text");

            builder.HasOne(x => x.TestOrder)
                   .WithMany(o => o.TestResults)
                   .HasForeignKey(x => x.TestOrderId);
        }
    }
}
