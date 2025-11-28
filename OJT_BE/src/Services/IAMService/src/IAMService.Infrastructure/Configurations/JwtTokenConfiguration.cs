using IAMService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMService.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Type Configuration for JwtToken
    /// </summary>
    public class JwtTokenConfiguration : IEntityTypeConfiguration<JwtToken>
    {
        public void Configure(EntityTypeBuilder<JwtToken> builder)
        {
            builder.ToTable("JwtToken");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasColumnType("text"); // Changed from HasMaxLength(500) to text type

            builder.Property(x => x.TokenType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("RefreshToken")
                .HasComment("Token type: RefreshToken or AccessToken");

            builder.Property(x => x.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => x.Token)
                .IsUnique()
                .HasDatabaseName("IX_JwtToken_Token");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_JwtToken_UserId");

            // Foreign key relationship
            builder.HasOne(x => x.User)
                .WithMany(u => u.JwtTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_JwtToken_User_UserId");
        }
    }
}