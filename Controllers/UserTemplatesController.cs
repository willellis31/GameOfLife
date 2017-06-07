using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebGameOfLife;
using WebGameOfLife.Attributes;

namespace WebGameOfLife.Controllers
{
    public class UserTemplatesController : Controller
    {
        private wdtentitiesEntities3 db = new wdtentitiesEntities3();

        // GET: UserTemplates
        public ActionResult Home()
        {
            return View();
        }

        // GET: UserTemplates
        public ActionResult Index(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var userTemplates = db.UserTemplates.Where(k => k.Name.Contains(name));
                var results = userTemplates.Include(u => u.User);
                return View(userTemplates.ToList());
            }
            else
            {
                var userTemplates = db.UserTemplates.Include(u => u.User);
                ViewBag.Title = "All Templates";
                return View(userTemplates.ToList());
            }

        }
        [Authorized]
        public ActionResult MyTemplates()
        {
            //Find our User instance from session
            var user = db.Users.Find(Session["UserID"]);

            //lets search for templates by UserID
            var userTemplates = db.UserTemplates.ToList();
            var mytemplates = from x in userTemplates
                              where x.UserID == user.UserID
                              select x;
            ViewBag.Title = "My Templates";
            return View("Index", mytemplates);

        }

        // GET: UserTemplates
        public ActionResult CreateGame(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            if (userTemplate == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("CreateFromTemplate", "UserGames", userTemplate);
        }

        // GET: UserTemplates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            if (userTemplate == null)
            {
                return HttpNotFound();
            }
            return View(userTemplate);
        }

        // GET: UserTemplates/Create
        [Authorized]
        public ActionResult Create()
        {
            //okay we need to find our UserID and set it
            var id = Session["UserID"];
            var list = new List<User>();
            var user = db.Users.Find(id);
            list.Add(user);

            ViewBag.UserID = new SelectList(list, "UserID", "Email");
            return View();
        }


        // GET: UserGames/Select Template
        public ActionResult SelectTemplate()
        {
            ViewBag.UserTemplateID = new SelectList(db.UserTemplates, "UserTemplateID", "Name");
            return View();
        }




        // POST: UserTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserTemplateID,UserID,Name,Height,Width,Cells")] UserTemplate userTemplate)
        {
            if (ModelState.IsValid)
            {
                //just to be sure we want all cells strings to be lower case
                userTemplate.Cells = CellsToLower(userTemplate.Cells);
                db.UserTemplates.Add(userTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userTemplate.UserID);
            return View(userTemplate);
        }

        // GET: UserTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            if (userTemplate == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userTemplate.UserID);
            return View(userTemplate);
        }

        // POST: UserTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserTemplateID,UserID,Name,Height,Width,Cells")] UserTemplate userTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userTemplate.UserID);
            return View(userTemplate);
        }

        // GET: UserTemplates/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            if (userTemplate == null)
            {
                return HttpNotFound();
            }
            return View(userTemplate);
        }

        // POST: UserTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            //check if we own the template
            if (userTemplate.UserID != (int)Session["UserID"])
            {
                return RedirectToAction("Index", "Home", 405);
            }
            db.UserTemplates.Remove(userTemplate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        public ActionResult TemplateRequest(int templateid, string name)
        {
            //find the template via entity framework
            UserTemplate userTemplate = db.UserTemplates.Find(templateid);

            //transform our cells value to 
            var html = TemplateToHTML(CellsToGrid(userTemplate));

            //return our updated game string
            Object returnObj = new { html = html };

            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }



        public string TemplateToHTML(List<List<char>> game)
        {
            //lets make a table from the 'cells' value
            StringBuilder returnstring = new StringBuilder();

            returnstring.Append("<table>");
            foreach (List<char> row in game)
            {
                returnstring.Append("<tr>");
                foreach (char cell in row)
                {
                    returnstring.Append("<td width='10' height='10'");
                    if (cell == 'x')
                        returnstring.Append(" style='background-color: black;'>");
                    else
                        returnstring.Append(">");
                    returnstring.Append("</td>");
                }
                returnstring.Append("</tr>");
            }
            returnstring.Append("</table>");


            return returnstring.ToString();
        }




        public List<List<char>> CellsToGrid(UserTemplate game)
        {
            //make our Cells a char array
            var letters = game.Cells.Trim().ToCharArray();
            //okay we are going to write a loop that will transform our cells string to a 2-d array

            //a list for our grid
            var matrix = new List<List<char>>();
            for (int y = 0; y < game.Height; y++)
            {
                //new row array
                var row = new List<char>();
                for (int x = 0; x < game.Width; x++)
                {
                    int index = (y * game.Width) + x;
                    row.Add(letters[index]);
                }
                matrix.Add(row);
            }
            return matrix;
        }


        public string CellsToLower(string cells)
        {
            StringBuilder cellsbuilder = new StringBuilder();

            foreach(char item in cells)
            {
                if (item == 'x' || item == 'X')
                    cellsbuilder.Append('x');
                if (item == 'o' || item == 'O')
                    cellsbuilder.Append('o');
            }
            return cellsbuilder.ToString();
        }


    }

}

