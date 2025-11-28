
namespace IAMService.Infrastructure.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;IAMService.Domain.Entities.Privilege&gt;" />
    public class PrivilegeConfiguration : IEntityTypeConfiguration<Privilege>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Privilege> builder)
        {
            builder.ToTable("Privilege");

            builder.HasKey(p => p.PrivilegeId);

            builder.Property(p => p.PrivilegeName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Description)
                   .HasMaxLength(200);


        }
    }
}
