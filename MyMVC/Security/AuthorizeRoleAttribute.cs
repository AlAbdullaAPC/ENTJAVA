using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyMVC.Models.DB;
using MyMVC.Models.EntityManager;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyMVC.Security
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] userAssignedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            this.userAssignedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool authorize = false;

            using (MyDBContext db = new MyDBContext())
            {
                UserManager um = new UserManager();
                foreach(var role in userAssignedRoles)
                {
                    authorize = um.IsUserInRole(context.HttpContext.User.Identity.Name, role);
                    if (authorize)
                        return;
                }
            }

            context.Result = new RedirectResult("~/Home/UnAuthorized");
        }
    }
}
