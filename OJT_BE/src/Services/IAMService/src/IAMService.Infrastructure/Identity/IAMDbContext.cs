


namespace IAMService.Infrastructure.Identity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class IAMDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IAMDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public IAMDbContext(DbContextOptions<IAMDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public DbSet<Role> Roles { get; set; }
        /// <summary>
        /// Gets or sets the privileges.
        /// </summary>
        /// <value>
        /// The privileges.
        /// </value>
        public DbSet<Privilege> Privileges { get; set; }
        /// <summary>
        /// Gets or sets the role privileges.
        /// </summary>
        /// <value>
        /// The role privileges.
        /// </value>
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        /// <summary>
        /// Gets or sets the refresh tokens.
        /// </summary>
        /// <value>
        /// The refresh tokens.
        /// </value>
        public DbSet<JwtToken> JwtTokens { get; set; }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run. However, it will still run when creating a compiled model.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
        /// examples.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
