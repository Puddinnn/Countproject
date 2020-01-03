using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CQCCRI.DBUtility;
using System.Text;
using System.Reflection;
using System.Collections;

public partial class Charts_GetPointCurveData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        int pointId = 0;
        //string starttime;
        //string finaltime;

        string pointIdString = Request["m_id"];
        string starttimeString = Request["start_time"];
        string finaltimeString = Request["final_time"];

        if (!Int32.TryParse(pointIdString, out pointId))
        {
            Response.Write("参数传递错误");
        }
        else
        {
            Response.Write(this.QueryPointDataById(pointId, starttimeString, finaltimeString));
        }

        Response.End();
    }


    private string QueryPointDataById(int pointId, string starttime, string finaltime)
    {
        SQLHelper helper = new SQLHelper();
        string sql = @" select     [D_ID],
            [M_ID],
            [D_Value],
            [D_ValueState_Name],
            CONVERT(VARCHAR,[D_Generated_Time], 120) AS D_Generated_Time from [Device_Data_Test] where m_id = '" + pointId.ToString() + "' AND D_Generated_Time BETWEEN '" + starttime + "'AND '" + finaltime + "'"; //------ m_id -> m_address
        DataTable dt = helper.ExecuteDataTable(sql);
        return this.DataTableToJsonString(dt);
    }





    //    int pointId = 0;

    //    string pointIdString = Request["m_id"];
    //    //string pointIdString = Request["m_address"];

    //    if (!Int32.TryParse(pointIdString, out pointId))
    //    {
    //        Response.Write("参数传递错误");
    //    }
    //    else
    //    {
    //        Response.Write(this.QueryPointDataById(pointId));
    //    }

    //    Response.End();
    //}

    //private string QueryPointDataById(int pointId)
    //{
    //    SQLHelper helper = new SQLHelper();

    //    string sql = @" select     [D_ID],
    //        [M_ID],
    //        [D_Value],
    //        [D_ValueState_Name],
    //        CONVERT(VARCHAR,[D_Generated_Time], 120) AS D_Generated_Time from [Device_Data_Test] where m_id = '" + pointId.ToString() + "'";
    //    DataTable dt = helper.ExecuteDataTable(sql);
    //    return this.DataTableToJsonString(dt);
    //}


    //------------------------------------------------------------------------------------------------------------------------------------------------


    //private string DataTableToJsonString(DataTable dt)
    //{
    //    StringBuilder sb = new StringBuilder();

    //    sb.Append("");

    //    List<string> xValues = new List<string>();
    //    List<string> yValues = new List<string>();

    //    if(dt !=null && dt.Rows.Count > 0)
    //    {
    //        for(int i=0;i<dt.Rows.Count;i++)
    //        {
    //            xValues.Add("'" + dt.Rows[i]["D_Generated_Time"] + "'");
    //            yValues.Add(dt.Rows[i]["D_Value"] + "");
    //        }
    //    }

    //    return sb.ToString();
    //}


    /// <summary>
    /// dataTable转换成Json格式
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    /// 

    public string DataTableToJsonString(DataTable dt)
    {
        if (dt != null && dt.Rows.Count > 0)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append("data");
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    //jsonBuilder.Append(jsonStringFromat(dt.Rows[i][j].ToString()));
                    jsonBuilder.Append((dt.Rows[i][j].ToString()));
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        else
        {
            return "";
        }
    }
 }