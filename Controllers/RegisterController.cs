using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGameOfLife;

namespace WebGameOfLife.Controllers
{
    public class RegisterController : Controller
    {
        private wdtentitiesEntities3 db = new wdtentitiesEntities3();

        // GET: Register/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Register/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Register/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Email,Password,FirstName,LastName,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                user.IsAdmin = false;
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);


                db.Users.Add(user);
                db.SaveChanges();

                //log our user in 
                Session.Add("UserEmail", user.Email);
                Session.Add("UserID", user.UserID);
                Session.Add("UserFirstName", user.FirstName);
                Session.Add("IsAdmin", false);
                Session.Add("Welcome", true);

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }
    }
}


       

