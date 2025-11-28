

namespace IAMService.Infrastructure.Seed
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Seeds the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        public static async Task SeedAsync(IAMDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { RoleCode = "ADMIN", RoleName = "Administrator", RoleDescription = "System administrator" },
                    new Role { RoleCode = "LAB_MANAGER", RoleName = "Lab Manager", RoleDescription = "Manage lab and users" },
                    new Role { RoleCode = "SERVICE", RoleName = "Service", RoleDescription = "Service management" },
                    new Role { RoleCode = "LAB_USER", RoleName = "Lab User", RoleDescription = "Perform testing and data input" },
                    new Role { RoleCode = "CUSTOM_ROLE", RoleName = "Custom Role", RoleDescription = "Custom user privileges" }
                };
                context.Roles.AddRange(roles);
            }

            if (!context.Privileges.Any())
            {
                var privileges = new List<Privilege>
                {
                    new Privilege { PrivilegeId = 1, PrivilegeName = "Read-only", Description = "View patient test orders and results" },
                    new Privilege { PrivilegeId = 2, PrivilegeName = "Create Test order", Description = "Create a new patient test order" },
                    new Privilege { PrivilegeId = 3, PrivilegeName = "Modify Test order", Description = "Modify patient test order" },
                    new Privilege { PrivilegeId = 4, PrivilegeName = "Delete Test order", Description = "Delete patient test order" },
                    new Privilege { PrivilegeId = 5, PrivilegeName = "Review Test order", Description = "Review test order result" },
                    new Privilege { PrivilegeId = 6, PrivilegeName = "Add comment", Description = "Add new comment" },
                    new Privilege { PrivilegeId = 7, PrivilegeName = "Modify comment", Description = "Modify comment" },
                    new Privilege { PrivilegeId = 8, PrivilegeName = "Delete comment", Description = "Delete comment" },
                    new Privilege { PrivilegeId = 9, PrivilegeName = "View configuration", Description = "View configurations" },
                    new Privilege { PrivilegeId = 10, PrivilegeName = "Create configuration", Description = "Add new configuration" },
                    new Privilege { PrivilegeId = 11, PrivilegeName = "Modify configuration", Description = "Modify configuration" },
                    new Privilege { PrivilegeId = 12, PrivilegeName = "Delete configuration", Description = "Delete configuration" },
                    new Privilege { PrivilegeId = 13, PrivilegeName = "View user", Description = "View all users" },
                    new Privilege { PrivilegeId = 14, PrivilegeName = "Create user", Description = "Create new user" },
                    new Privilege { PrivilegeId = 15, PrivilegeName = "Modify user", Description = "Modify user" },
                    new Privilege { PrivilegeId = 16, PrivilegeName = "Delete user", Description = "Delete user" },
                    new Privilege { PrivilegeId = 17, PrivilegeName = "Lock/Unlock user", Description = "Lock or unlock user" },
                    new Privilege { PrivilegeId = 18, PrivilegeName = "View role", Description = "View roles" },
                    new Privilege { PrivilegeId = 19, PrivilegeName = "Create role", Description = "Create new custom role" },
                    new Privilege { PrivilegeId = 20, PrivilegeName = "Update role", Description = "Modify privileges of role" },
                    new Privilege { PrivilegeId = 21, PrivilegeName = "Delete role", Description = "Delete custom role" },
                    new Privilege { PrivilegeId = 22, PrivilegeName = "View Event Logs", Description = "View event logs" },
                    new Privilege { PrivilegeId = 23, PrivilegeName = "Add Reagents", Description = "Add new reagents" },
                    new Privilege { PrivilegeId = 24, PrivilegeName = "Modify Reagents", Description = "Modify reagent information" },
                    new Privilege { PrivilegeId = 25, PrivilegeName = "Delete Reagents", Description = "Delete reagents" },
                    new Privilege { PrivilegeId = 26, PrivilegeName = "Add Instrument", Description = "Add new instrument" },
                    new Privilege { PrivilegeId = 27, PrivilegeName = "View Instrument", Description = "View and check instrument status" },
                    new Privilege { PrivilegeId = 28, PrivilegeName = "Activate/Deactivate Instrument", Description = "Activate or deactivate instrument" },
                    new Privilege { PrivilegeId = 29, PrivilegeName = "Execute Blood Testing", Description = "Perform blood testing" }
                };
                context.Privileges.AddRange(privileges);
            }

            if (!context.RolePrivileges.Any())
            {
                var rolePrivileges = new List<RolePrivilege>();

                // Admin có tất cả quyền
                for (int i = 1; i <= 29; i++)
                    rolePrivileges.Add(new RolePrivilege { RoleCode = "ADMIN", PrivilegeId = i });

                // Các role khác như mình đã định nghĩa ở trên
                int[] labManager = { 1, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 25, 26, 27, 28 };
                int[] service = { 9, 10, 11, 12, 22, 23, 24, 25, 26, 27, 28, 29 };
                int[] labUser = { 1, 2, 3, 4, 5, 6, 7, 8, 22, 23, 24, 25, 26, 27, 28, 29 };
                int[] customRole = { 1 };

                foreach (var id in labManager)
                    rolePrivileges.Add(new RolePrivilege { RoleCode = "LAB_MANAGER", PrivilegeId = id });

                foreach (var id in service)
                    rolePrivileges.Add(new RolePrivilege { RoleCode = "SERVICE", PrivilegeId = id });

                foreach (var id in labUser)
                    rolePrivileges.Add(new RolePrivilege { RoleCode = "LAB_USER", PrivilegeId = id });

                foreach (var id in customRole)
                    rolePrivileges.Add(new RolePrivilege { RoleCode = "CUSTOM_ROLE", PrivilegeId = id });

                context.RolePrivileges.AddRange(rolePrivileges);
            }

            // ✅ Seed user mặc định (Admin)
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    RoleCode = "ADMIN",
                    Email = EncryptionHelper.EncryptDeterministic("admin@system.com"),
                    PhoneNumber = EncryptionHelper.Encrypt("0123456789"),
                    FullName = EncryptionHelper.Encrypt("System Admin"),
                    IdentifyNumber = EncryptionHelper.Encrypt("000000000"),
                    Gender = "Male",
                    Age = 30,
                    Address = EncryptionHelper.Encrypt("System HQ"),
                    DateOfBirth = new DateTime(1995, 1, 1),
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                };
                context.Users.Add(adminUser);
            }

            await context.SaveChangesAsync();
        }
    }
}