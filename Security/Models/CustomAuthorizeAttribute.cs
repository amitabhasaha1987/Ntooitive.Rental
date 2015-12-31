using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Security.Models
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                var authorizedUsers = ConfigurationManager.AppSettings[UsersConfigKey];
                var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];

                Users = String.IsNullOrEmpty(Users) ? authorizedUsers : Users;
                Roles = String.IsNullOrEmpty(Roles) ? authorizedRoles : Roles;

                if (!String.IsNullOrEmpty(Roles))
                {
                    if (!CurrentUser.IsInRole(Roles))
                    {
                        filterContext.Result = new RedirectResult("/unauthorize/permission-denied");
                        // base.OnAuthorization(filterContext); //returns to login url
                    }
                }

                if (!String.IsNullOrEmpty(Users))
                {
                    if (!Users.Contains(CurrentUser.UserId.ToString()))
                    {
                        filterContext.Result = new RedirectResult("/unauthorize/permission-denied");
                    }
                }
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var principal = (HttpContext.Current.User as CustomPrincipal);
            filterContext.Result = principal != null ? new RedirectResult("/unauthorize/permission-denied") : new RedirectResult("/agent/agent-login");
        }
    }

}
