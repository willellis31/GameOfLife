using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebGameOfLife
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if(Session["User"] != null)
                {
                    if ((bool)Session["IsAdmin"] == true)
                    {
                        //admin user is already logged in.
                        Server.Transfer("AdminHome.aspx", false);
                    }
                }
                
            }
                
                
            if (Page.IsPostBack)
            {
                using (var connection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\David\\Documents\\assignment2wdt.mdf; Integrated Security = True; MultipleActiveResultSets = True; Connect Timeout = 30; Application Name = EntityFramework"))
                {
                    connection.Open();

                    var dataTable = new DataTable();

                    var adapter = new SqlDataAdapter("SelectAllUsers", connection);
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row[1].ToString() == TextBoxEmail.Text)
                        {
                            //we have a matching username.
                            if((bool)row[5] == true)
                            {
                                //we are an admin user
                                if (BCrypt.Net.BCrypt.Verify(TextBoxPwd.Text, row[2].ToString()))
                                {
                                    
                                    //add user session data.
                                    Session.Add("UserEmail", row[1].ToString());
                                    Session.Add("UserID", (int)row[0]);
                                    Session.Add("UserFirstName", row[3].ToString());
                                    Session.Add("IsAdmin", (bool)row[5]);
                                    // password matches then transfer them.
                                    Server.Transfer("AdminHome.aspx", true);
                                }
                            }
                                
                        }
                    }
                    LabelResult.Text = "Username or Password incorrect.";
                }
                //check if we have a matching login or not.
               //Login1.UserName
               //Login1.Password
            }
        }
    }
}