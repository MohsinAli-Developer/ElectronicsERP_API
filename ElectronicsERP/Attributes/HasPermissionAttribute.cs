using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace ElectronicsERP.Attributes
{
    // Custom authorization attribute to check permission claims in JWT
    public class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _permission;
        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // if user is not logged in or doesn’t have required permission
            if (!user.Identity.IsAuthenticated ||
                !user.HasClaim(c => c.Type == "permission" && c.Value == _permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
