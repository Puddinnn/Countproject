using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Charts_PointCurve : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextBox1.Text = DateTime.Now.ToString("yyyy-MM-01");
        this.TextBox2.Text = DateTime.Now.ToString("yyyy-MM-dd");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home_page.aspx");
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("Sign_In.aspx");
    }
}