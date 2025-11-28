using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedData(PatientDbContext context, ILogger logger)
        {
            try
            {
                // Đảm bảo database đã được tạo
                await context.Database.EnsureCreatedAsync();

                // Chỉ seed data nếu bảng FlaggingSetConfigs đang trống
                if (!await context.FlaggingSetConfigs.AnyAsync())
                {
                    await SeedFlaggingSetConfigs(context);
                    logger.LogInformation("Seeding FlaggingSetConfig data finished.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedFlaggingSetConfigs(PatientDbContext context)
        {
            var flaggingConfigs = new List<FlaggingSetConfig>
            {
                // White Blood Cell Count (WBC)
                new FlaggingSetConfig { TestName = "White Blood Cell Count (WBC)", LowThreshold = 4000f, HighThreshold = 10000f },
                
                // Red Blood Cell Count (RBC) - Gender specific
                new FlaggingSetConfig { TestName = "Red Blood Cell Count (RBC) (Male)", LowThreshold = 4.7f, HighThreshold = 6.1f },
                new FlaggingSetConfig { TestName = "Red Blood Cell Count (RBC) (Female)", LowThreshold = 4.2f, HighThreshold = 5.4f },

                // Hemoglobin (Hb/HGB) - Gender specific
                new FlaggingSetConfig { TestName = "Hemoglobin (Hb/HGB) (Male)", LowThreshold = 14f, HighThreshold = 18f },
                new FlaggingSetConfig { TestName = "Hemoglobin (Hb/HGB) (Female)", LowThreshold = 12f, HighThreshold = 16f },

                // Hematocrit (HCT) - Gender specific
                new FlaggingSetConfig { TestName = "Hematocrit (HCT) (Male)", LowThreshold = 42f, HighThreshold = 52f },
                new FlaggingSetConfig { TestName = "Hematocrit (HCT) (Female)", LowThreshold = 37f, HighThreshold = 47f },

                // Platelet Count (PLT)
                new FlaggingSetConfig { TestName = "Platelet Count (PLT)", LowThreshold = 150000f, HighThreshold = 350000f },

                // Mean Corpuscular Volume (MCV)
                new FlaggingSetConfig { TestName = "Mean Corpuscular Volume (MCV)", LowThreshold = 80f, HighThreshold = 100f },

                // Mean Corpuscular Hemoglobin (MCH)
                new FlaggingSetConfig { TestName = "Mean Corpuscular Hemoglobin (MCH)", LowThreshold = 27f, HighThreshold = 33f },

                // Mean Corpuscular Hemoglobin Concentration (MCHC)
                new FlaggingSetConfig { TestName = "Mean Corpuscular Hemoglobin Concentration (MCHC)", LowThreshold = 32f, HighThreshold = 36f },
            };

            await context.FlaggingSetConfigs.AddRangeAsync(flaggingConfigs);
            await context.SaveChangesAsync();
        }
    }
}