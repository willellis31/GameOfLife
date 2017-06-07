using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebGameOfLife.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(int? id)
        {
            if(id == 404)
                ViewBag.Redirect = true;
            if (id == 405)
                ViewBag.Permission = true;
            return View();
        }
        public RedirectResult Admin()
        {
            return Redirect("../AdminHome.aspx");
        }
    }
}