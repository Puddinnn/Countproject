/*----------------------------------------------------------------
 * Copyright (C) 2009 重庆煤炭科学研究院测控分院
 * 版权所有。 
 *
 * 文件名：SQLHelper.cs
 * 文件功能描述：封装对MSSQL Server 2005的基本操作,包括查询/添加/删除/更新等.
 * 
 * 
 * 
 * 创建标识：陈运启@20091020
 * 
 * 
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CQCCRI.DBUtility
{
    /// <summary>
    /// 封装对 MSSQL Server 2005的基本操作,以SQL语句/参数/存储过程三种方式提供这些基本操作方法. 
    /// </summary>
    public class SQLHelper
    {
        #region private valiable
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string connectionString;// = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

        /// <summary>
        ///数据库执行命令 
        /// </summary>
        private SqlCommand com = new SqlCommand();

        /// <summary>
        /// 数据库连接
        /// </summary>
        private SqlConnection con = new SqlConnection();
        #endregion private valiable

        #region construction 

        /// <summary>
        /// 构造函数,使用Web.config中默认的数据库连接字符串 SQLConnectionString 
        /// </summary>
        public SQLHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            this.com.CommandTimeout = 90;
        }


        /// <summary>
        /// 构造函数,使用参数connectionString做为连接字符串
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public SQLHelper(string connectionString)
        {
            this.connectionString = connectionString;
            this.com.CommandTimeout = 90;
        }

        /// <summary>
        /// 构造函数,根据传入参数
        /// </summary>
        /// <param name="connectionStringOrName">当前传入的连接字符串值(connectionString)或连接连接字符串名称(name)</param>
        /// <param name="isName">如果传入的是连接字符串名称(name),此值为真；否则为假，表示连接字符串</param>
        public SQLHelper(string connectionStringOrName, bool isName)
        {
            if (isName)
            {
                this.connectionString = ConfigurationManager.ConnectionStrings[connectionStringOrName].ConnectionString;
            }

            else
            {
                this.connectionString = connectionStringOrName;
            }
        }
        #endregion construction

        #region private functions

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private void OpenConnection()
        {
            if (con.State != ConnectionState.Open)
            {
                con.ConnectionString = connectionString;
                com.Connection = con;
                con.Open();
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        private void CloseConnection()
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// 发现存储过程的所有Input参数,并赋予相对应的参数值.对于所有
        /// </summary>
        /// <param name="returnValue">表示,是否需要返回存储过程默认的第一个结果值.</param>
        /// <param name="values">参数值列表</param>
        private void DiscoverParameters(bool returnValue, object[] values)
        {
            com.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(com);

            //删除存储过程的第一个参数
            if (!returnValue)
            {
                com.Parameters.RemoveAt(0);
            }
            int temp = 0;

            //给每个参数赋值
            for (int i = 0, j = com.Parameters.Count; i < j; i++)
            {

                //参数类型不是input类型,则赋空值
                if (com.Parameters[i].Direction == ParameterDirection.InputOutput || com.Parameters[i].Direction == ParameterDirection.Output)
                {
                    temp++;
                    com.Parameters[i].Value = DBNull.Value;
                }
                else //给input类型参数赋值
                {
                    com.Parameters[i].Value = values[i - temp];
                }
            }
        }

        /// <summary>
        /// 用正则表达式的方式,获得SQL语句中的参数(以@"开头"),并给每个参数赋值
        /// </summary>
        /// <param name="parametersValues">参数值列表</param>
        private void AssignValues(params object[] parametersValues)
        {
            //SQL语句为空,返回
            if (string.IsNullOrEmpty(com.CommandText))
            {
                throw new Exception("SQL语句不完整");
            }

            //匹配SQL语句中的参数
            Regex reg = new Regex(@"@[\w]+");
            MatchCollection mc = reg.Matches(com.CommandText);
            if (mc.Count != parametersValues.Length)
            {
                throw new Exception("SQL语句请求与给予参数个数不同");
            }
            com.Parameters.Clear();

            //给每个参数赋值
            if (mc.Count > 0)
            {
                for (int i = 0, j = mc.Count; i < j; i++)
                {
                    com.Parameters.AddWithValue(mc[i].Value, parametersValues[i]);
                }
            }
        }

        #endregion private functions

        #region public methods

        /// <summary>
        /// 执行SQL语句查询,以object类型,返回查询结果的第一行,第一列,忽略其它行列.
        /// </summary>
        /// <param name="commandText">要执行查询的SQL语句</param>
        /// <returns>返回执行查询结果首行首列</returns>
        public object ExecuteScalar(string commandText)
        {
            object retVal = null;

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                retVal = com.ExecuteScalar();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            return retVal;
        }

        /// <summary>
        /// 以参数方式执行SQL查询语句,返回查询结果的首行首列.
        /// </summary>
        /// <param name="commandText">要执行查询的SQL语句,其中参数以@开关</param>
        /// <param name="values">参数列表集合</param>
        /// <returns>查询结果首行首列</returns>
        public object ExecuteParaScalar(string commandText, params object[] values)
        {
            object retVal = null;

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                AssignValues(values);
                retVal = com.ExecuteScalar();
                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return retVal;
        }

        /// <summary>
        /// 执行存储过程,返回查询结果的道行首列.
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="values">存储过程参数值列表</param>
        /// <returns>返回查询首行首列</returns>
        public object ExecuteSpScalar(string commandText, params object[] values)
        {
            object retVal = null;

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.StoredProcedure;
                DiscoverParameters(false, values);
                retVal = com.ExecuteScalar();
                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return retVal;
        }

        /// <summary>
        /// 执行SQL语句,以DataSet结果集方式,返回查询结果.
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <returns>返回查询结果集</returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            DataSet ds = new DataSet();

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                SqlDataAdapter sda = new SqlDataAdapter(com);
                if (sda == null)
                {
                    ds = null;
                }
                else
                {
                    sda.Fill(ds);
                }
                CloseConnection();

            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 执行带参数的SQL语句,以DataSet结果集方式,返回查询结果.
        /// </summary>
        /// <param name="commandText">带参数的SQL语句</param>
        /// <param name="values">参数值列表</param>
        /// <returns>查询结果集</returns>
        public DataSet ExecuteParaDataSet(string commandText, params object[] values)
        {
            DataSet ds = new DataSet();

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                AssignValues(values);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                if (sda == null)
                {
                    ds = null;
                }
                else
                {
                    sda.Fill(ds);
                }
                CloseConnection();

            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 执行存储过程,以DataSet结果集方式,返回查询结果.
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="values">存储过程参数值列表</param>
        /// <returns>查询结果集</returns>
        public DataSet ExecuteSpDataSet(string commandText, params object[] values)
        {
            DataSet ds = new DataSet();

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.StoredProcedure;
                DiscoverParameters(false, values);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                if (sda == null)
                {
                    ds = null;
                }
                else
                {
                    sda.Fill(ds);
                }
                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 执行查询SQL语句,以数据表DataTable的形式,返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <returns>查询结果数据表DataTable</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            DataSet ds = ExecuteDataSet(commandText);
            if (ds == null || ds.Tables.Count < 1)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 执行带参数的SQL语句,以DataTable形式,返回查询结果.
        /// </summary>
        /// <param name="commandText">带参数的SQL语句</param>
        /// <param name="values">参数值列表</param>
        /// <returns>查询结果的数据表DataTable</returns>
        public DataTable ExecuteParaDataTable(string commandText, params object[] values)
        {
            DataSet ds = ExecuteParaDataSet(commandText, values);
            if (ds == null || ds.Tables.Count < 1)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 执行存储过程,以DataTable形式,返回查询结果.
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="values">存储过程参数值列表</param>
        /// <returns>查询结果的数据表DataTable</returns>
        public DataTable ExecuteSpDataTable(string commandText, params object[] values)
        {
            DataSet ds = ExecuteSpDataSet(commandText, values);
            if (ds == null || ds.Tables.Count < 1)
                return null;
            else
                return ds.Tables[0];
        }

        /// <summary>
        /// 执行插入/更新SQL语句,并返回执行影响的条数
        /// </summary>
        /// <param name="commandText">插入/更新SQL语句</param>
        /// <returns>执行SQL语句所影响的条数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            int retVal = -1;

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                retVal = com.ExecuteNonQuery();
                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return retVal;
        }

        /// <summary>
        /// 执行带参数的插入/更新SQL语句,并返回语句执行所影响的记录的条数
        /// </summary>
        /// <param name="commandText">带参数的插入/更新SQL语句</param>
        /// <param name="values">参数值列表</param>
        /// <returns>执行SQL语句所影响的条数</returns>
        public int ExecuteParaNonQuery(string commandText, params object[] values)
        {
            int retVal = -1;
            OpenConnection();

            try
            {
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                AssignValues(values);
                retVal = com.ExecuteNonQuery();

                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            return retVal;
        }

        /// <summary>
        /// 执行插入/更新数据的存储过程,并返回存储过程执行所影响的记录的条数
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="values">存储过程参数值列表</param>
        /// <returns>执行存储过程所影响的条数</returns>
        public int ExecuteSpNonQuery(string commandText, params object[] values)
        {
            int retVal = -1;
            OpenConnection();
            try
            {
                com.CommandText = commandText;
                com.CommandType = CommandType.StoredProcedure;
                DiscoverParameters(false, values);
                retVal = com.ExecuteNonQuery();

                CloseConnection();
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return retVal;
        }

        /// <summary>
        /// 执行SQL语句,并返回查询结果所组成的只读记录集(SqlDataReader)
        /// </summary>
        /// <param name="commandText">要执行的SQL语句</param>
        /// <returns>查询所得记录集</returns>
        public SqlDataReader ExecuteDataReader(string commandText)
        {
            SqlDataReader sdr = null;

            try
            {
                OpenConnection();

                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                sdr = com.ExecuteReader(CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return sdr;
        }

        /// <summary>
        /// 执行带参数的SQL语句,并返回查询结果所组成的只读记录集(SqlDataReader)
        /// </summary>
        /// <param name="commandText">要执行的带参数的SQL语句</param>
        /// <param name="values">参数值列表</param>
        /// <returns>查询所得记录集</returns>
        public SqlDataReader ExecuteParaDataReader(string commandText, params object[] values)
        {
            SqlDataReader sdr = null;
            try
            {
                OpenConnection();

                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                AssignValues(values);
                sdr = com.ExecuteReader(CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return sdr;
        }

        /// <summary>
        /// 执行存储过程,并返回查询结果所组成的只读记录集(SqlDataReader)
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="values">存储过程参数值列表</param>
        /// <returns>查询所得记录集</returns>
        public SqlDataReader ExecuteSpDataReader(string commandText, params object[] values)
        {
            SqlDataReader sdr = null;

            try
            {
                OpenConnection();
                com.CommandText = commandText;
                com.CommandType = CommandType.Text;
                com.CommandType = CommandType.StoredProcedure;
                DiscoverParameters(false, values);
                sdr = com.ExecuteReader(CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return sdr;
        }
        #endregion public methods
    }

}
