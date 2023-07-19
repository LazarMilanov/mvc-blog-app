using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBlogApp.Models;
using MVCBlogApp.AuthorizeFilters;
using MVCBlogApp.Extensions;
using System.Net;
using System.Data.Entity;

namespace MVCBlogApp.Controllers
{
    public class PostsController : Controller
    {
        private MVCBlogEntities db = new MVCBlogEntities();

        // GET: Posts
        [LoggedOrAuthorized("Posts", "Create")]
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePostViewModel model) {
            if (ModelState.IsValid) {
                Post post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    DatePosted = DateTime.Today
                };
                var userID = (int)HttpContext.Session["UserID"];
                post.Author = db.Users.Single(u => u.UserID == userID);
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index", "Home").Success("Your post has been created!");
            }
            return View(model);               
        }

        [LoggedOrAuthorized("Posts", "Edit")]
        public ActionResult Edit(int id)
        {
            var post = db.Posts.Find(id);
            if (post.Author.UserID != (int)HttpContext.Session["UserID"])
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (post == null)
            {
                return HttpNotFound();
            }
            EditPostViewModel postInfo = new EditPostViewModel();
            postInfo.Title = post.Title;
            postInfo.Content = post.Content;
            return View(postInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EditPostViewModel model) {
            var post = db.Posts.Find(id);
            if (post == null) {
                return HttpNotFound();
            }           
            if (ModelState.IsValid) {
                post.Title = model.Title;
                post.Content = model.Content;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home").Success("Post successfully edited");
            }
            return View(post);
        }

        [AllowAnonymous]
        public ActionResult Details(int id) {
            var post = db.Posts.SingleOrDefault(p => p.PostID == id);
            return View(post);
        }

        [AllowAnonymous]
        public ActionResult UserPosts(string username) {
            if (username == null || username == "") {
                return new EmptyResult();
            }
            if (!db.Users.Where(u => u.Username == username).Any()) {
                return Content("<h2>User not found</h2>");
            }
            var posts = from p in db.Posts
                        where p.Author.Username == username
                        orderby p.PostID
                        select p;
            int count = 0;
            foreach (var item in posts)
            {
                count++;
            }
            ViewBag.User = username;
            ViewBag.PostCount = count;
            return View(posts);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            var post = db.Posts.Find(id);
            if ((int)HttpContext.Session["UserID"] != post.Author.UserID || HttpContext.Session["UserID"] == null) {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index", "Home").Success("Post successfully deleted");
        }

        [ChildActionOnly]
        public ActionResult LatestPostsPartial() {
            var posts = db.Posts.OrderByDescending(p => p.PostID).Take(4);
            return PartialView(posts);
        }
    }
}