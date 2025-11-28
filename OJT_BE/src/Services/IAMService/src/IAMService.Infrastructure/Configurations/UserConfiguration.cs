
namespace IAMService.Infrastructure.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;IAMService.Domain.Entities.User&gt;" />
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.DateOfBirth)
                   .HasColumnType("timestamp without time zone");

            builder.Property(u => u.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(u => u.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.IdentifyNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(u => u.Gender)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleCode)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.JwtTokens)
                   .WithOne(rt => rt.User)
                   .HasForeignKey(rt => rt.UserId);
        }
    }
}
