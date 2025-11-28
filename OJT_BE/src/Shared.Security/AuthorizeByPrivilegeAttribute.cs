using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Security
{
    /// <summary>
    /// selectively authorizes access to controllers or actions based on user privileges.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeByPrivilegeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// The required privilege
        /// </summary>
        private readonly string _requiredPrivilege;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeByPrivilegeAttribute"/> class.
        /// </summary>
        /// <param name="requiredPrivilege">The required privilege.</param>
        public AuthorizeByPrivilegeAttribute(string requiredPrivilege)
        {
            _requiredPrivilege = requiredPrivilege;
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var privileges = user.Claims
                                 .Where(c => c.Type == "Privilege")
                                 .Select(c => c.Value)
                                 .ToList();

            if (!privileges.Contains(_requiredPrivilege))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
