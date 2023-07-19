using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogApp.AuthorizeFilters
{
    public class LoggedOrAuthorizedAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string NextController { get; set; }
        public string NextAction { get; set; }

        public LoggedOrAuthorizedAttribute(string nextcontroller, string nextaction)
        {
            NextController = nextcontroller;
            NextAction = nextaction;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            { 
                return;
            }
 
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login?nextcontroller=" + NextController + "&nextaction=" + NextAction);
            }
        }
    }
}