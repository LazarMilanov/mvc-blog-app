using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCBlogApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "{page}",
                defaults: new { controller = "Home", action = "Index", page = 1 }
            );

            routes.MapRoute(
                name: "Home2",
                url: "Home/Index/{page}",
                defaults: new { controller = "Home", action = "Index", page = 1 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "Account/Login/{nextcontroller}/{nextaction}",
                defaults: new { controller = "Posts", action = "UserPosts", nextcontroller = UrlParameter.Optional, nextaction = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UserPosts",
                url: "Posts/UserPosts/{username}",
                defaults: new { controller = "Posts", action = "UserPosts", username = UrlParameter.Optional }
            );
        }
    }
}
