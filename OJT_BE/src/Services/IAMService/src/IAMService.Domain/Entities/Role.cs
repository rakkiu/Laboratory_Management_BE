
namespace IAMService.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        /// <value>
        /// The role code.
        /// </value>
        public string RoleCode { get; set; } = null!;
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; } = null!;
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        /// <value>
        /// The role description.
        /// </value>
        public string RoleDescription { get; set; } = null!;

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        public ICollection<User> Users { get; set; } = new HashSet<User>();
        /// <summary>
        /// Gets or sets the role privileges.
        /// </summary>
        /// <value>
        /// The role privileges.
        /// </value>
        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new HashSet<RolePrivilege>();
    }
}
