using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities.Patient;


namespace PatientService.Infrastructure.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.PatientId);

            // Changed to text type for encrypted data storage
            builder.Property(p => p.FullName).IsRequired().HasColumnType("text");
            builder.Property(p => p.Email).IsRequired().HasColumnType("text");
            builder.Property(p => p.PhoneNumber).IsRequired().HasColumnType("text");
            builder.Property(p => p.Address).HasColumnType("text");
            builder.Property(p => p.IdentifyNumber).IsRequired().HasColumnType("text");
            
            builder.Property(p => p.Gender).HasMaxLength(10);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            // Thêm unique index cho PhoneNumber, Email và IdentifyNumber
            builder.HasIndex(p => p.PhoneNumber).IsUnique();
            builder.HasIndex(p => p.Email).IsUnique();
            builder.HasIndex(p => p.IdentifyNumber).IsUnique();

            builder.HasMany(p => p.MedicalRecords)
                   .WithOne(r => r.Patient)
                   .HasForeignKey(r => r.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}