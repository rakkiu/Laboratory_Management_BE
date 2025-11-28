
namespace IAMService.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Privilege
    {
        /// <summary>
        /// Gets or sets the privilege identifier.
        /// </summary>
        /// <value>
        /// The privilege identifier.
        /// </value>
        public int PrivilegeId { get; set; }
        /// <summary>
        /// Gets or sets the name of the privilege.
        /// </summary>
        /// <value>
        /// The name of the privilege.
        /// </value>
        public string PrivilegeName { get; set; } = null!;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the role privileges.
        /// </summary>
        /// <value>
        /// The role privileges.
        /// </value>
        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new HashSet<RolePrivilege>();
    }
}
