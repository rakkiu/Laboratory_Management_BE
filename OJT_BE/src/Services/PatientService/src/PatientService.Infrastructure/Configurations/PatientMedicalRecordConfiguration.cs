using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Domain.Entities.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Configurations
{
    public class PatientMedicalRecordConfiguration : IEntityTypeConfiguration<PatientMedicalRecord>
    {
        public void Configure(EntityTypeBuilder<PatientMedicalRecord> builder)
        {
            builder.ToTable("PatientMedicalRecords");

            builder.HasKey(r => r.RecordId);

            builder.Property(r => r.IsDeleted).HasDefaultValue(false);

            // Add this line to configure Version as a concurrency token
            builder.Property(r => r.Version).IsConcurrencyToken();

            builder.HasMany(r => r.AuditLogs)
                   .WithOne(a => a.Record)
                   .HasForeignKey(a => a.RecordId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
