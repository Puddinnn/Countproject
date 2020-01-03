using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CQCCRI.DBUtility;
/// <summary>
/// 业务逻辑处理模块
/// </summary>
public class BusinessProcess
{
    public BusinessProcess()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    
    public static DataTable QueryDeviceInfoList(string Point, string startingtime, string finishingtime)
    {
        string sql;
        //string sql = "select * from Device_Data_Test";    
        if (Point == "全部")
        {
            sql = "SELECT Device_Info_Test.M_Code as '测点编号', Device_Info_Test.M_Address as '测点位置', Device_Data_Test.D_Value as '监测值', Device_Data_Test.D_ValueState_Name as '状态'," +
                   "Device_Data_Test.D_Generated_Time as '采集时间'FROM Device_Info_Test INNER JOIN Device_Data_test ON Device_Info_test.M_ID = Device_Data_test.M_ID WHERE D_Generated_Time BETWEEN '" + startingtime + "'" + " AND '" + finishingtime + "'";
        }

        else
        {
            sql = "SELECT Device_Info_Test.M_Code as '测点编号', Device_Info_Test.M_Address as '测点位置', Device_Data_Test.D_Value as '监测值', Device_Data_Test.D_ValueState_Name as '状态'," +
                   "Device_Data_Test.D_Generated_Time as '采集时间' FROM Device_Info_Test INNER JOIN Device_Data_test ON Device_Info_test.M_ID = Device_Data_test.M_ID WHERE M_Address='" + Point + "' AND D_Generated_Time BETWEEN '" + startingtime + "'" +
                   " AND '" + finishingtime + "'";
        }

        SQLHelper mh = new SQLHelper();

        return mh.ExecuteDataTable(sql);
    }
}