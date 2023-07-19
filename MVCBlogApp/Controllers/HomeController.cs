using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBlogApp.Models;
using System.Data.Entity;

namespace MVCBlogApp.Controllers
{
    public class HomeController : Controller
    {
        private MVCBlogEntities db = new MVCBlogEntities();

        public ActionResult Index(int page)
        {
            int pageSize = 4;
            var posts = db.Posts.OrderByDescending(p => p.PostID).Skip((page-1)*pageSize).Take(pageSize);
            float a = db.Posts.Count() / (float)pageSize;
            var pageCount = Math.Ceiling(a);
            ViewBag.pageCount = pageCount;
            ViewBag.currentPage = page; 
            return View(posts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Hello and welcome to my MVC Blog app! Hope you'll enjoy it!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Here should be my contact";

            return View();
        }
    }
}