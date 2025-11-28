using Microsoft.EntityFrameworkCore;
using PatientService.Application.Interfaces;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Data
{
    public class PatientDbContext : DbContext, IApplicationDbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<PatientMedicalRecord> PatientMedicalRecords => Set<PatientMedicalRecord>();
        public DbSet<PatientRecordAuditLog> PatientRecordAuditLogs => Set<PatientRecordAuditLog>();

        // ✅ Thêm các DbSet của Test Order Service
        public DbSet<TestOrder> TestOrders => Set<TestOrder>();
        public DbSet<TestResult> TestResults => Set<TestResult>();
        public DbSet<TestResultDetail> TestResultDetails => Set<TestResultDetail>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<FlaggingSetConfig> FlaggingSetConfigs => Set<FlaggingSetConfig>();
        public DbSet<TestOrderAuditLog> TestOrderAuditLogs => Set<TestOrderAuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 👇 Dùng chung schema
            modelBuilder.HasDefaultSchema("patient");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
