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
    public class PatientRecordAuditLogConfiguration : IEntityTypeConfiguration<PatientRecordAuditLog>
    {
        public void Configure(EntityTypeBuilder<PatientRecordAuditLog> builder)
        {
            builder.ToTable("PatientRecordAuditLogs");

            builder.HasKey(a => a.AuditId);
            builder.Property(a => a.Action).HasMaxLength(50).IsRequired();
        }
    }
}
