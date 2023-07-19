using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using MVCBlogApp.Models;
using MVCBlogApp.AuthorizeFilters;
using MVCBlogApp.Extensions;
using System.Net;
using System.Net.Mail;
using System.Data.Entity;
using Jose;
using System.IO;

namespace MVCBlogApp.Controllers
{
    public class AccountController : Controller
    {
        // MVCBlogEntities db = new MVCBlogEntities();
        // GET: Account
        public ActionResult Register()
        {
            if (HttpContext.Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            using (MVCBlogEntities db = new MVCBlogEntities()) { 
                if (ModelState.IsValid) {
                    // Create user object
                    User user = new User();
                    // Get username
                    user.Username = model.Username;
                    // Get email
                    user.Email = model.Email;
                   
                    var checkUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
                    var checkEmail = db.Users.FirstOrDefault(u => u.Email == user.Email);

                    // Check if a user with that username and/or email exists
                    if (checkUser != null && checkEmail != null) {
                        ModelState.AddModelError("", "A user with that username and email already exists");
                        return View(model);
                    }
                    if (checkUser != null) {
                        ModelState.AddModelError("", "A user with that username already exists");
                        return View(model);
                    }
                    if (checkEmail != null)
                    {
                        ModelState.AddModelError("", "A user with that email already exists");
                        return View(model);
                    }

                    // Create salt
                    var salt = HashingProcessor.CreateSalt();
                    user.PasswordSalt = salt;

                    // Generate Hash with input and salt
                    user.Password = HashingProcessor.GenerateHash(model.Password, salt);

                    // Add the user to database and save

                    db.Users.Add(user);
                    db.SaveChanges();

                    return RedirectToAction("Login", "Account").Success("Your account has been created! You may now log in.");
                }
                // If this line is reached, something went wrong
                return View(model);
            }
        }

        // HTTP GET
        // If the user is already logged in, return them to the homepage
        public ActionResult Login(string nextcontroller, string nextaction)
        {
            if (HttpContext.Session["UserID"] != null) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string nextcontroller, string nextaction, LoginViewModel model)
        {
            using (MVCBlogEntities db = new MVCBlogEntities()) {
                // Look for the user with the entered email
                var loginUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (ModelState.IsValid)
                {
                    // Check if the user exists
                    if (loginUser != null)
                    {
                        // Check if the password is correct
                        if (HashingProcessor.AreEqual(model.Password, loginUser.Password, loginUser.PasswordSalt) == true)
                        {
                            // Add a session object and data
                            Session["UserID"] = loginUser.UserID;
                            Session["Username"] = loginUser.Username;
                            Session["Email"] = loginUser.Email;
                            Session["ProfileImage"] = loginUser.ProfileImage;
                            Session["Posts"] = loginUser.Posts;

                            if (nextcontroller != null || nextcontroller != "") {
                                return RedirectToAction(nextaction, nextcontroller).Success("Welcome " + HttpContext.Session["Username"]);
                            }
                            return RedirectToAction("Index", "Home").Success("Welcome " + HttpContext.Session["Username"]);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Wrong password");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user with that email doesn't exist");
                        return View(model);
                    }
                }
                // If this line is reached, something went wrong
                return View(model);
            }            
        }

        public ActionResult Logout() {
            // Destroy the session object
            Session.Abandon();
            return RedirectToAction("Index", "Home").Information("You were logged out");
        }

        [LoggedOrAuthorized("Account", "ProfileInfo")]
        public ActionResult ProfileInfo() {
            return View();
        }

        [LoggedOrAuthorized("Account", "UpdateInfo")]
        public ActionResult UpdateInfo()
        {
            using (MVCBlogEntities db = new MVCBlogEntities()) {
                /*if ( id != (int)HttpContext.Session["UserID"]) {
                    return Content("<h2>403 Forbidden</h2>");
                }*/
                var id = (int)HttpContext.Session["UserID"];
                var user = db.Users.Find(id);

                UserUpdateViewModel userInfo = new UserUpdateViewModel();
                if (user == null)
                {
                    return HttpNotFound();
                }

                userInfo.Username = user.Username;
                userInfo.Email = user.Email;

                return View(userInfo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(UserUpdateViewModel model)
        {
            using (MVCBlogEntities db = new MVCBlogEntities()) {
                var id = (int)HttpContext.Session["UserID"];
                var user = db.Users.Find(id);

                if (user == null)
                {
                    return HttpNotFound();
                }
                if (ModelState.IsValid) {
                    user.Email = model.Email;
                    HttpContext.Session["Email"] = model.Email;
                    user.Username = model.Username;
                    HttpContext.Session["Username"] = model.Username;
                    if (model.ProfileImage != null) {
                        string[] fileParams = model.ProfileImage.FileName.Split('.');
                        
                        WebImage img = new WebImage(model.ProfileImage.InputStream);
                        if (img.Width > 250 || img.Height > 250) {
                            img.Resize(250, 250, false);
                        }

                        string newFileName;
                        string pathToSave;
                        do
                        {
                            newFileName = HashingProcessor.RandomTokenHex() + "." + fileParams[1];
                            pathToSave = HttpContext.Server.MapPath("~/Static/profile_pics/" + newFileName);

                        } while (System.IO.File.Exists(pathToSave));
                        /*if (System.IO.File.Exists(pathToSave))
                        {
                            newFileName = HashingProcessor.RandomTokenHex() + "." + fileParams[1];
                            pathToSave = HttpContext.Server.MapPath("~/Static/profile_pics/" + newFileName);
                        }*/
                        img.Save(pathToSave);
                        user.ProfileImage = "~/Static/profile_pics/" + newFileName;
                        HttpContext.Session["ProfileImage"] = "~/Static/profile_pics/" + newFileName;
                    }                    
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ProfileInfo").Success("Info successfully changed");
                }
                return View(user);
            }
        }

        public ActionResult ResetRequest() {
            if (HttpContext.Session["UserID"] != null) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetRequest(ResetRequestViewModel model) {
            using (MVCBlogEntities db = new MVCBlogEntities()) { 
                if (ModelState.IsValid) {
                    var user = db.Users.First(u => u.Email == model.Email);
                    SendResetMail(user);
                    return RedirectToAction("Login", "Account").Information("Check your email and click the link");
                }
            }
            return View(model);
        }

        public ActionResult ResetPassword(string token) {
            try
            {
                if (token.Last().ToString() == "a") {
                    string newToken = token.Remove(token.Length - 1);
                    return RedirectToAction("ResetPassword", "Account", new { token = newToken });
                }
                string idstring = JWT.Decode<Dictionary<string, string>>(token, null, JwsAlgorithm.none)["sub"];
                int id = int.Parse(idstring);
                if (idstring == null || idstring == "") {
                    return Content("<h2>That is an invalid or expired token.</h2>");
                }
                return View();
            }
            catch (Exception)
            {
                return Content("<h2>That is an invalid or expired token or maybe an error occured.</h2>");
            }                                  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string token, ResetPasswordViewModel model)
        {
            using (MVCBlogEntities db = new MVCBlogEntities()) {
                int id = int.Parse(JWT.Decode<Dictionary<string, string>>(token, null, JwsAlgorithm.none)["sub"]);
                if (ModelState.IsValid) {
                    User user = db.Users.Find(id);
                    if (user == null) {
                        return Content("<h2>That is an invalid or expired token.</h2>");
                    }
                    var salt = HashingProcessor.CreateSalt();
                    user.PasswordSalt = salt;
                    user.Password = HashingProcessor.GenerateHash(model.Password, salt);
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account").Success("Your password was successfully changed!");
                }
            }
            return new EmptyResult();
        }

        [NonAction]
        public void SendResetMail(User user) {
            const int expireSeconds = 3600;
            // DateTime exp = (DateTime.Now + TimeSpan.FromMinutes(expireMinutes));

            // Creating the token
            var payload = new Dictionary<string, string>();
            payload.Add("sub", user.UserID.ToString());
            // Expiry time (must be in Unix seconds)
            payload.Add("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expireSeconds).ToString());
            string token = JWT.Encode(payload, null, JwsAlgorithm.none);

            // Send email
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("noreply@demo.com", "MVC Blog");
            mail.To.Add(user.Email);
            mail.Subject = "Password Reset";
            mail.Body = "To reset your password, visit the following link: http://localhost:16655/Account/ResetPassword?token=" + token + "a" + Environment.NewLine + "If you did not make this request then simply ignore this email and no changes will be made." + Environment.NewLine + Environment.NewLine + "MVC Blog";

            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("lazar.milanov.bg@gmail.com",
                Environment.GetEnvironmentVariable("GMAIL_PASS", EnvironmentVariableTarget.User));
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }
    }
}