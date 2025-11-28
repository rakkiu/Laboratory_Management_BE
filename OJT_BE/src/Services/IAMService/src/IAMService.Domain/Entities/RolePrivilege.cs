

namespace IAMService.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class RolePrivilege
    {
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        /// <value>
        /// The role code.
        /// </value>
        public string RoleCode { get; set; } = null!;
        /// <summary>
        /// Gets or sets the privilege identifier.
        /// </summary>
        /// <value>
        /// The privilege identifier.
        /// </value>
        public int PrivilegeId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public Role Role { get; set; } = null!;
        /// <summary>
        /// Gets or sets the privilege.
        /// </summary>
        /// <value>
        /// The privilege.
        /// </value>
        public Privilege Privilege { get; set; } = null!;
    }
}
