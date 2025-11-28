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
    public class TestOrderAuditLogConfiguration : IEntityTypeConfiguration<TestOrderAuditLog>
    {
        public void Configure(EntityTypeBuilder<TestOrderAuditLog> builder)
        {
            builder.ToTable("TestOrderAuditLog");

            builder.HasKey(x => x.LogId);

            builder.Property(x => x.UserId)
                   .HasColumnType("text"); // ✅ TEXT thay vì VARCHAR

            builder.Property(x => x.ActionType)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(x => x.ChangedFields)
                   .HasColumnType("text");
        }
    }
}
