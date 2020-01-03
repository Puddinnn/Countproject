using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CQCCRI.DBUtility;
/// <summary>
/// 业务逻辑处理模块
/// </summary>
public class SpecialValueCal
{
    public SpecialValueCal()
    {

    }

    public static DataTable QueryDeviceInfoList(string Point, string Select ,string startingtime, string finishingtime)
    {
        string sql;
        string sql_Combine, sql_Renew;
        //string sql = "select * from Device_Data_Test";

        sql_Combine = "INSERT INTO tempdb_test (mcode, maddress, mtype, dvalue, dtime) " +
            "SELECT M_code, M_address, M_Type_Name, D_Value, D_Generated_Time FROM Device_Info_Test JOIN Device_Data_Test ON Device_Data_Test.M_ID = Device_Info_Test.M_ID";

        sql_Renew = "delete tempdb_test where id not in( select max(id) from tempdb_test group by mcode, dvalue, dtime )";

        if (Select == "max")
        {
            sql = "select mcode as '测点编号', maddress as '测点地址', mtype as '检测类型', dvalue as '最大值', dtime as '最值出现时间'" +
                  " from tempdb_test where id not in (select a.id from tempdb_test a, tempdb_test b where a.dvalue < b.dvalue and a.mcode = b.mcode)" +
                  " and maddress = '" + Point + "' and dtime between'" + startingtime + "'" + " AND '" + finishingtime + "'";
        }

        else if(Select == "min")
        {
            sql = "select mcode as '测点编号', maddress as '测点地址', mtype as '检测类型', dvalue as '最小值', dtime as '最值出现时间'" +
                  " from tempdb_test where id not in (select a.id from tempdb_test a, tempdb_test b where a.dvalue > b.dvalue and a.mcode = b.mcode)" +
                  " and maddress = '" + Point + "' and dtime between'" + startingtime + "'" + " AND '" + finishingtime + "'";
        }

        else
        {
            sql = "select mcode as '测点编号',maddress as '测点地址', AVG(dvalue) as '平均值' from tempdb_test where maddress = '"+Point+ "' and dtime between'" + startingtime + "'" + " AND '" + finishingtime + "' GROUP BY mcode, maddress ";
        }

        SQLHelper mh = new SQLHelper();

        mh.ExecuteDataTable(sql_Combine);
        mh.ExecuteDataTable(sql_Renew);

        return mh.ExecuteDataTable(sql);
    }
}