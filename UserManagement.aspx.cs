using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebGameOfLife
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Authorisation process.
            if (Session["UserID"] != null)
            {
                //user is logged on, check if admin
                var user = (bool)Session["IsAdmin"];
                if (user == false)
                    Server.Transfer("Admin.aspx", true);
            }
            else
            {
                //logged on user is not admin
                Server.Transfer("Admin.aspx", true);
            }
        }

        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            Server.Transfer("AdminHome.aspx", true);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //asked me to define do I need it?
        }
    }
}