using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebGameOfLife.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessLogin(User user)
        {
            string email = user.Email;
            //We search the database for matching record?

        var db = new wdtentitiesEntities3();
        var users = db.Users.ToList();

            var u = from x in users
                    where x.Email == user.Email
                    select x;

            if (u.Count() == 0)
            {
                //No matching user
                ModelState.AddModelError("", "The user login or password provided is incorrect.");
                return View("Index");
            }
                
            else
            {
                //we have found the user
                //compare passwords (hash first)
                var password = u.First().Password;
               
                if (BCrypt.Net.BCrypt.Verify(user.Password, password))
                {
                    //success, save username to sessions
                    Session.Add("UserEmail", user.Email);
                    Session.Add("UserID", u.First().UserID);
                    Session.Add("UserFirstName", u.First().FirstName);
                    Session.Add("IsAdmin", u.First().IsAdmin);
                    return RedirectToAction("Index", "Home"); ;
                }
                ModelState.AddModelError("", "The user login or password provided is incorrect.");
                return View("Index");
            }

        }
        public ActionResult Logout()
        {
            Session.Clear();
            return View("../Home/Index");
        }
    }
}