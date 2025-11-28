namespace IAMService.Application.Models.Role
{
    /// <summary>
    /// Role Data Transfer Object
    /// </summary>
    public class RoleDTO
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; }
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        /// <value>
        /// The role code.
        /// </value>
        public string RoleCode { get; set; }
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        /// <value>
        /// The role description.
        /// </value>
        public string RoleDescription { get; set; }

    }
}
