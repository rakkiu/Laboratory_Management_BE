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
    public class TestOrderConfiguration : IEntityTypeConfiguration<TestOrder>
    {
        public void Configure(EntityTypeBuilder<TestOrder> builder)
        {
            builder.ToTable("TestOrder");

            builder.HasKey(x => x.TestOrderId);

            builder.Property(x => x.PatientName)
                   .HasColumnType("text") // TEXT thay vì VARCHAR
                   .IsRequired();

            builder.Property(x => x.Gender)
                   .HasColumnType("text");

            builder.Property(x => x.Address)
                   .HasColumnType("text");

            builder.Property(x => x.PhoneNumber)
                   .HasColumnType("text");

            builder.Property(x => x.Status)
                   .HasColumnType("text")
                   .HasDefaultValue("Pending");

            // ⚙️ Các field người dùng → TEXT
            builder.Property(x => x.CreatedBy)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(x => x.RunBy)
                   .HasColumnType("text");

            builder.Property(x => x.ReviewedBy)
                   .HasColumnType("text");
        }
    }
}
