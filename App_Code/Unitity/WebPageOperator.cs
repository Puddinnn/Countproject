using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 


/// <summary>
/// WebPageOperator 的摘要说明
/// </summary>
public static class WebPageOperator
{

    public static void ShowPromptTip(System.Web.UI.Page page, string tipText)
    {
        page.Response.Write("<script>alert('"+tipText+"')</script>");
    }

}