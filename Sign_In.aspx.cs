using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Sign_In : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string user_ID = ID_Content.Text;
        string user_Pwd = Pwd_Content.Text;

        //先判断输入是否规范
        if(string.IsNullOrEmpty(user_ID) || string.IsNullOrEmpty(user_Pwd))
        {
            WebPageOperator.ShowPromptTip(this, "用户名不能为空，请重新输入!");
            return;
        }

        //再判断用户名密码是否正确
        if(UserInfo.CheckUserIsValid(user_ID,user_Pwd))
        {
            Response.Redirect("Home_page.aspx");
            return;
        }
        else
        {
            WebPageOperator.ShowPromptTip(this, "用户名或密码输入不正确，请重新输入！");
            return;
        }

        /*

        if(!String.IsNullOrEmpty(user_ID))
        {
            if(user_ID == "test")
            {
                if(user_Pwd == "test")
                {
                    Response.Redirect("Home_page.aspx");
                }
                else
                {
                    Response.Write("<script>alert('密码输入错误，请重新输入！')</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('用户名不存在，请重新输入！')</script>");
            }
        }

        else
        {
            Response.Write("<script> alert('用户名不能为空，请重新输入！')</script>");
        }
        */
    }
}