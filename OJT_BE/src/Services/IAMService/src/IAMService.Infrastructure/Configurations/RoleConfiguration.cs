
namespace IAMService.Infrastructure.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;IAMService.Domain.Entities.Role&gt;" />
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role");

            builder.HasKey(r => r.RoleCode);

            builder.Property(r => r.RoleCode)
                   .HasMaxLength(50);

            builder.Property(r => r.RoleName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.RoleDescription)
                   .HasMaxLength(200);


        }
    }
}
