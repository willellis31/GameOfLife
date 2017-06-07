using Newtonsoft.Json;
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
using WebGameOfLife.Models;

namespace WebGameOfLife.Controllers
{
    public class UserGamesController : Controller
    {
        private wdtentitiesEntities3 db = new wdtentitiesEntities3();


        public ActionResult Home()
        {
            return View();
        }


        [Authorized]
        public ActionResult SavedGames()
        {
            //Find our User instance from session
            var user = db.Users.Find(Session["UserID"]);

            //lets search for templates by UserID
            var userGames = db.UserGames.ToList();
            var mygames = from x in userGames
                              where x.UserID == user.UserID
                              select x;
            ViewBag.Title = "My Saved Games";
            return View("Index", mygames);
        }


        // GET: UserGames
        public ActionResult ActiveGamesIndex()
        {
            var userGames = (List<UserGame>)Session["Games"];
            ViewBag.Title = "My Active Games";
            return View("Index", userGames);
        }

        // GET: UserGames/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGame userGame = db.UserGames.Find(id);
            if (userGame == null)
            {
                return HttpNotFound();
            }
            return View(userGame);
        }


        // GET: UserGames/Details/5
        public ActionResult Play(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            
            if(Session["Games"] == null)
            {
                //This means that haven't actually got a session list of games yet, lets create it.
                Session.Add("Games", new List<UserGame>());
            }
            List<UserGame> Games = (List<UserGame>)Session["Games"];
            var dbGame = db.UserGames.Find(id);

            if (dbGame == null)
            {
                UserGame userGame = Games.Find(x => x.UserGameID == id);

                //get the html value of the game grid
                ViewBag.gamegrid = GameToHTML(CellsToGrid(userGame));
                if (userGame == null)
                {
                    return HttpNotFound();
                }
                return View(userGame);
            }

            else
            {
                //We have loaded our game, but we need to add it to our active games.
                ViewBag.gamegrid = GameToHTML(CellsToGrid(dbGame));

                //lets add our game to the session.

                //first check if the game has a matching UserGameID
                bool cnt = true;
                while (cnt)
                {
                    var matchinggameid = from x in Games
                                       where x.UserGameID == dbGame.UserGameID
                                       select x;
                    if (matchinggameid.Count() > 0)
                        dbGame.UserGameID++;
                    else
                        cnt = false;
                }
                Games.Add((UserGame)dbGame);
                return View(dbGame);
            }
        }
        

        // GET: UserGames/Create

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewGame(int UserTemplateID)
        {
            ViewBag.Email = Session["UserEmail"];
            ViewBag.LoggedOnUser = Session["UserID"];
            if (ViewBag.LoggedOnUser == null)
                ViewBag.LoggedOnUser = 1;
            ViewBag.TemplateID = UserTemplateID;


            //get our template
            var temp = db.UserTemplates.Find(UserTemplateID);
            ViewBag.Template = temp;

            UserGame Item = new UserGame();
            Item.UserID = ViewBag.LoggedOnUser;
            //Set some default values
            Item.Height = temp.Height;
            Item.Width = temp.Width;

            
            

            ViewBag.HTMLcells = GameToHTML(CellsToGrid(ViewBag.Template));

            return View("Create", Item);
        }

        public ActionResult CreateFromTemplate(int UserTemplateID)
        {
            ViewBag.Email = Session["UserEmail"];
            ViewBag.LoggedOnUser = Session["UserID"];
            if (ViewBag.LoggedOnUser == null)
                ViewBag.LoggedOnUser = 1;
            ViewBag.TemplateID = UserTemplateID;


            //get our template
            var temp = db.UserTemplates.Find(UserTemplateID);
            ViewBag.Template = temp;

            UserGame Item = new UserGame();
            Item.UserID = ViewBag.LoggedOnUser;
            //Set some default values
            Item.Height = temp.Height;
            Item.Width = temp.Width;

            ViewBag.HTMLcells = GameToHTML(CellsToGrid(ViewBag.Template));

            return View("Create", Item);
        }

        // POST: UserGames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserGameID,UserID,Name,Height,Width,Cells")] UserGame userGame)
        {
            if (ModelState.IsValid)
            {
                //we not going to save games automatically to the database, we are going to save them in the session.

                //check if the List has been instantiated
                if(Session["Games"] == null)
                {
                    Session.Add("Games", new List<UserGame>());
                    //if we just instantiated we can add to the list straight away
                    //add to our list
                    List<UserGame> Games = (List<UserGame>)Session["Games"];
                    Games.Add(userGame);
                }
                else
                {
                    //we need to find out what UserGameIDs exist in the Session List
                    //if the Session variable games has not been set no games exist in the session yet.
                    //cast our variable to a list then.
                    List<UserGame> Games = (List<UserGame>)Session["Games"];
                    
                    //Occassionally we may have a list that is empty, becuase all the games have been deleted
                    //so check the list is not empty...
                    if (Games.Count > 0)
                    {
                        //here we will loop through the list to make sure we are not re-using an existing 
                        //UserGameId
                        int x = 0;
                        bool useusergameid = false;
                        while (!useusergameid)
                        {
                            var matchingUserGameID = from gm in Games
                                                     where gm.UserGameID == x
                                                     select new { gm.UserGameID };
                            if(matchingUserGameID.ToList().Count > 0)
                                x++;
                            else
                            {
                                useusergameid = true;
                                userGame.UserGameID = x;
                            }

                        }
                    }
                    Games.Add(userGame);
                }
                return RedirectToAction("ActiveGamesIndex");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userGame.UserID);
            return View(userGame);
        }

        [HttpPost]
        public ActionResult TakeTurn(UserGame jsonData)
        {
            
            //make our string into a grid
            var originalgame = CellsToGrid(jsonData);

            //take a turn
            var updatedgame = TakeGameTurn(originalgame, jsonData.UserGameID);

            //Add the string Cells to Session
            Session.Add("Cells", GameToString(updatedgame));

            //return our updated game string
            Object returnObj = new { val = GameToString(updatedgame), html = GameToHTML(updatedgame) };

            return Json(returnObj, JsonRequestBehavior.AllowGet );
        }


        // GET: UserGames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGame userGame = db.UserGames.Find(id);
            if (userGame == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userGame.UserID);
            return View(userGame);
        }

        // POST: UserGames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserGameID,UserID,Name,Height,Width,Cells")] UserGame userGame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userGame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Email", userGame.UserID);
            return View(userGame);
        }

        // Delete session games
        public ActionResult DeleteActive(int? id)
        {
            //retrive session game data.
            var userGames = (List<UserGame>)Session["Games"];

            //select the one that has the gameid
            var result = from x in userGames
                            where x.UserGameID == id
                            select x;

            //Get the first result
            var userSessionGame = result.First();
            ViewBag.gamegrid = GameToHTML(CellsToGrid(userSessionGame));
            ViewBag.type = "active";
            return View("Delete", userSessionGame);
        }

        // POST: UserGames/DeleteActive
        [HttpPost, ActionName("DeleteActive")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteActiveConfirmed(int id)
        {

            var userGames = (List<UserGame>)Session["Games"];

            //select the one that has the gameid
            var sessionGame = from x in userGames
                            where x.UserGameID == id
                            select x;

            userGames.Remove(sessionGame.First());

            return RedirectToAction("ActiveGamesIndex");

        }


        // GET: UserGames/Delete/5
        public ActionResult DeleteDB(int? id)
        {
            var userDBGame = db.UserGames.Find(id);
            ViewBag.gamegrid = GameToHTML(CellsToGrid(userDBGame));
            ViewBag.type = "db";
            return View("Delete", userDBGame);

        }


        // POST: UserGames/Delete/5
        [HttpPost, ActionName("DeleteDB")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDBConfirmed(int id)
        {
            var dbGame = db.UserGames.Find(id);
            db.UserGames.Remove(dbGame);
            db.SaveChanges();
            return RedirectToAction("SavedGames");

        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<List<char>> CellsToGrid(UserGame game)
        {
            //make our Cells a char array
            var letters = game.Cells.Trim().ToCharArray();
            //okay we are going to write a loop that will transform our cells string to a 2-d array

            //a list for our grid
            var matrix = new List<List<char>>();
            for(int y = 0; y < game.Height; y++)
            {
                //new row array
                var row = new List<char>();
                for(int x = 0; x < game.Width; x++)
                {
                    int index = (y * game.Width) + x;
                    row.Add(letters[index]);
                }
                matrix.Add(row);
            }
            return matrix;
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



        public List<List<char>> TakeGameTurn(List<List<char>> game, int gameid)
        {
            int height = game.Count;
            int width = game[0].Count;
            var updatedboard = new List<List<char>>();
            //we iterate over every y value
            for(int y =0; y< height; y++)
            {
                var row = new List<char>();

                //okay since we iterate over all the neighbours, we are using another loop, except we don't want to move out of the range or the original list/array
                int miny = y;
                int maxy = y;
                
                //min value for y range can be 0 no less (obviously)
                if ((miny - 1) >= 0)
                    miny--;
                //max value for y range cannot be greater than the game height
                if ((maxy + 1) < height)
                    maxy++;

                //iterate over every x value
                for(int x = 0; x < width; x++)
                {

                    int minx = x;
                    int maxx = x;
                    //the minx value cannot be less than 0
                    if ((minx - 1) >= 0)
                        minx--;
                    //cannot be wider than the game
                    if ((maxx + 1) < width)
                        maxx++;

                    //using these ranges, which come from the original cells we are iterating we iterate over its neighbours counting live neighbours.
                    Counter liveneighbours = new Counter();
                    liveneighbours.count = 0;
                    for (int neighby = miny; neighby <= maxy; neighby++)
                    {
                        for (int neighbx = minx; neighbx <= maxx; neighbx++)
                        {
                            //only count live neighbours
                            var cellvalue = game[neighby][neighbx];
                            if (cellvalue == 'o' && (neighby != y || neighbx != x))
                            {
                                liveneighbours.increment();
                            }
                        }
                    }
                    //if original cell alive
                    switch(game[y][x])
                    {
                        case 'o':
                            //it dies if we have less than 2 live neihbours dies
                            //it dies if it has more than 3 live neighbours
                            if (liveneighbours.count >= 2 && liveneighbours.count <= 3)
                                row.Add('o');
                            else
                                row.Add('x');
                            break;
                        case 'x':
                            //only we have exactly three live neighbours does a dead live reanimate.
                            if (liveneighbours.count == 3)
                                row.Add('o');
                            else
                                row.Add('x');
                            break;
                    }
                    
                }
                updatedboard.Add(row);
            }
            //update the game in the session
            UpdateSessionGame(GameToString(updatedboard), gameid);

            //pass our updated board back to the page.
            return updatedboard;
        }

        /* 
            UpdateSessionGame(string cells, int gameid) - finds our current game in the session
            from gameid, and sets its Cells property to cells.
         */
        public void UpdateSessionGame(string cells, int gameid)
        {
            //retrive our session game data.
            var userGames = (List<UserGame>)Session["Games"];

            //select the one that has the gameid
            var result = from x in userGames
                       where x.UserGameID == gameid
                       select x;
            //There will only be one result so just select it.
            var game = result.First();

            //update game.Cells value with our parameter
            game.Cells = cells;

            //replace the one in the session with the updated one.
        }


        public string GameToString(List<List<char>> game)
        {
            StringBuilder returnstring = new StringBuilder();
            foreach(List<char> row in game)
            {
                foreach(char cell in row)
                {
                    returnstring.Append(cell);
                }
            }
            return returnstring.ToString();
        }



        public string GameToHTML(List<List<char>> game)
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
                    if(cell == 'x')
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



        [HttpPost]
        public String CreateAndInsertTemplate(int templateID, int templateX, int templateY, int gameHeight,  int gameWidth)
        {
            //first we need to create a blank game string
            var gamestring = CreateBoard(gameHeight, gameWidth);

            var template = db.UserTemplates.Find(templateID);

            //make a usergame instance from it
            var game = new UserGame();
            game.Height = gameHeight;
            game.Width = gameWidth;
            game.Cells = gamestring;

            //then less make it into a 2-d grid

            var gamegrid = CellsToGrid(game);

            //inserting a template is as 'easy' as copying the values of the template to the corresponding gamegrid
            //and taking into account the offset values when doing so.
            //first check if the template will fit into the game at the specified coordinates.
            if (gameWidth < template.Width + templateX || gameHeight < template.Height + templateY)
                throw new ArgumentException("The game is not large enough for the template + offset values.");
            int x, y;

            //make template grid into 2d array

            var tempgame = new UserGame();
            tempgame.Cells = template.Cells;
            tempgame.Height = template.Height;
            tempgame.Width = template.Width;

            //make template cellstring into a grid
            var tempgrid = CellsToGrid(tempgame);

            //the for loop will interate over the GameOfLife.Cells thus the x and y values must be offset
            //our iteration will be as wide and as tall as the template. we can
            for (y = 0; y < template.Height; y++)
            {
                for (x = 0; x < template.Width; x++)
                {
                    gamegrid[y + templateY][x + templateX] = tempgrid[y][x];
                }
            }

            //make our tempgame string into a string and return it
            return GameToString(gamegrid);

        }


        public string CreateBoard(int height, int width)
        {
            StringBuilder gameCells = new StringBuilder();
            int x = height * width;
            for(int y = 0; y<x; y++)
                gameCells.Append('x');
            return gameCells.ToString();
            
        }

        [HttpPost]
        public ActionResult SaveGame(UserGame data)
        {
            // Create a new UserGame
            UserGame saveGame = new UserGame();
            // Save gamestate data to new instance on UserGame
            saveGame.UserGameID = data.UserGameID;
            saveGame.UserID = data.UserID;
            saveGame.Cells = (string)(Session["Cells"] );
            saveGame.Height = data.Height;
            saveGame.Width = data.Width;
            saveGame.Name = data.Name;

            //we need to find the logged on user.
            var user = db.Users.Find(Session["UserID"]);
            saveGame.User = user;

            // Add saveGame to database
            db.UserGames.Add(saveGame);
            db.SaveChanges();

            Object returnObj = new {Result = true };

            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
    }
}
