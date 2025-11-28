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
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");

            builder.HasKey(x => x.CommentId);

            builder.Property(x => x.UserName)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(x => x.Content)
                   .HasColumnType("text")
                   .IsRequired();

            builder.HasOne(x => x.TestOrder)
                   .WithMany(o => o.Comments)
                   .HasForeignKey(x => x.TestOrderId);

            builder.HasOne(x => x.TestResult)
                   .WithMany(r => r.Comments)
                   .HasForeignKey(x => x.ResultId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
