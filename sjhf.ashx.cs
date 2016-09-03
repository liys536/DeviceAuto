using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace DeviceAuto
{
    /// <summary>
    /// sjhf 的摘要说明
    /// </summary>
    public class sjhf : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string action = context.Request["action"];
            switch (action)
            {
                case "query":
                    Query();
                    break;
                case "restore":
                    Restore();
                    break;
            }
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        public void Query()
        {
            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时


                string sql = "delete from bflb";
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();


                string dirPath = ConfigurationManager.ConnectionStrings["bakpath"].ConnectionString;
                if (Directory.Exists(dirPath))
                {
                    //获得目录信息
                    DirectoryInfo dir = new DirectoryInfo(dirPath);
                    //获得目录文件列表
                    FileInfo[] files = dir.GetFiles("*.bak");
                    //string[] fileNames = new string[files.Length];
                    //int i = 0;
                    foreach (FileInfo fileInfo in files)
                    {
                        //fileNames[i] = fileInfo.Name;
                        //i++;

                        SqlParameter[] parms = {
                            new SqlParameter("@bfwj",fileInfo.Name),
                                 };

                        SqlCommand com = new SqlCommand();
                        com.Connection = con;//获取数据连接
                        com.Transaction = sqltra;//，在执行SQL时
                        com.CommandText = "insert_bflb";
                        com.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter p in parms)
                        {
                            com.Parameters.Add(p);
                        }
                        com.ExecuteNonQuery();
                    }

                    sqltra.Commit();        //提交事务

                    if (con.State == ConnectionState.Open) con.Close();
                    con.Dispose();
                }

                string strWhere = "1=1";

                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                DataSet duser = SqlHelper.GetList("bflb", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from bflb");
                string str = string.Empty;
                //将数据转换成json格式
                str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                HttpContext.Current.Response.Write(str);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        public void Restore()
        {
            try
            {

                //根据连接字符串获取数据库名称
                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                string db = strConn.Split(';')[1];
                string dbname = db.Split('=')[1];
                string bkname =HttpContext.Current.Request["data"].ToString();

                string bakpath = ConfigurationManager.ConnectionStrings["bakpath"].ConnectionString;
                string bkpath = bakpath + bkname;    //使用单个符号“\”提示常量中有换行符

                string connString = SqlHelper.conString;
                connString = connString.Replace(dbname, "master");        //替换数据库 
                SqlConnection con = new SqlConnection(connString);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;     //设置永不超时，否则恢复时间长容易产生超时错误
                cmd.CommandText = "exec p_RestoreDb '" + bkpath + "','" + dbname + "'";
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("数据恢复成功！");
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}