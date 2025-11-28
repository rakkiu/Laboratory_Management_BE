
namespace IAMService.Infrastructure.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration&lt;IAMService.Domain.Entities.RolePrivilege&gt;" />
    public class RolePrivilegeConfiguration : IEntityTypeConfiguration<RolePrivilege>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolePrivilege");

            builder.HasKey(rp => new { rp.RoleCode, rp.PrivilegeId });

            builder.HasOne(rp => rp.Role)
                   .WithMany(r => r.RolePrivileges)
                   .HasForeignKey(rp => rp.RoleCode)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Privilege)
                   .WithMany(p => p.RolePrivileges)
                   .HasForeignKey(rp => rp.PrivilegeId)
                   .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
