using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebGameOfLife
{
    public partial class WebForm2 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            

            //Authorisation process.
            if(Session["UserID"] != null)
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


        protected void ButtonUser_Click1(object sender, EventArgs e)
        {
            Server.Transfer("UserManagement.aspx", false);
        }

        protected void ButtonTemplate_Click1(object sender, EventArgs e)
        {
            Server.Transfer("TemplateManagement.aspx", false);
        }

        protected void ButtonLogout_Click(object sender, EventArgs e)
        {
            //Clear session variables.
            Session.Abandon();
            Server.Transfer("Admin.aspx", false);
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (FileUpload1.HasFile)
                {
                    using (var connection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\David\\Documents\\assignment2wdt.mdf; Integrated Security = True; MultipleActiveResultSets = True; Connect Timeout = 30; Application Name = EntityFramework"))
                    {
                        connection.Open();
                        //okay we need to read from the file.
                        FileUpload1.SaveAs(Server.MapPath("~/") + FileUpload1.FileName);

                        try
                        {
                            string[] lines = File.ReadAllLines(Server.MapPath("~/") + FileUpload1.FileName);

                            //Set the name for our new Template object
                            string name = FileUpload1.FileName;

                            //set our height
                            int height;
                            int.TryParse(lines[0], out height);

                            //set our width
                            int width;
                            int.TryParse(lines[0], out width);

                            //we need to piece our cells string together
                            StringBuilder cellsbuilder = new StringBuilder();
                            for(int x = 2; x < height +2; x++)
                            {
                                //we need to make all letters lower case.
                                foreach(char letter in lines[x])
                                {
                                    cellsbuilder.Append(char.ToLower(letter));
                                }
                            }

                            //cast back to string
                            string cells = cellsbuilder.ToString();
                            
                            //check that the cellsbuilder length is the right length
                            if(cells.Length != (height * width))
                            {
                                throw new Exception();
                            }

                            //make the database changes.

                            //make a new adapter with our stored procedure name as command.
                            var adapter = new SqlDataAdapter("InsertNewTemplate", connection);

                            //set command type to stored procedure.
                            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                            //set parameters for our stored procedure.
                            adapter.SelectCommand.Parameters.Add(new SqlParameter("@UserID", (int)Session["UserID"]));
                            adapter.SelectCommand.Parameters.Add(new SqlParameter("@Name", name));
                            adapter.SelectCommand.Parameters.Add(new SqlParameter("@Height", height));
                            adapter.SelectCommand.Parameters.Add(new SqlParameter("@Width", width));
                            adapter.SelectCommand.Parameters.Add(new SqlParameter("@Cells", cells));
                            DataTable dt = new DataTable();
                            //Execute
                            adapter.Fill(dt);
                        }
                        catch
                        {
                            //Oh no something is not right.
                            LabelUpload.Text = "File has errors.";
                        }


                        


                    }
                }
                LabelUpload.Text = "Template succesfully inserted!";
            }
            else
            {
                LabelUpload.Text = "You have not specified a file.";
            }
        }
    }
}