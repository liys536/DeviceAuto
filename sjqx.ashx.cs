using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DeviceAuto
{
    /// <summary>
    /// sjqx 的摘要说明
    /// </summary>
    public class sjqx : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string s = context.Request["data"];
            switch (action)
            {
                case "query":
                    Query(s);
                    break;
                case "edit":
                    Edit();
                    break;
            }
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string ygid)
        {
            try
            {
                string userid = HttpContext.Current.Request.Form["userid"];
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select depid from DataAuthority where userid='" + userid + "'");
                string depid = "";
                if (dt.Rows.Count > 0)
                {
                    depid=dt.Rows[0][0].ToString();
                }

                //string str = string.Empty;
                ////将数据转换成json格式
                //str = JSonHelper.CreateJsonParameters(dt, true, dt.Rows.Count);
                HttpContext.Current.Response.Write(depid);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        private void Edit()
        {
            try
            {
                string userid = HttpContext.Current.Request.Form["userid"];
                string depid = HttpContext.Current.Request.Form["depid"];
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from DataAuthority where userid='" + userid + "'");
                if (dt.Rows.Count > 0)
                {
                    string updatesql = "update DataAuthority set depid='" + depid + "' where userid='" + userid.Trim() + "';";
                    ExecSql(updatesql.ToString());
                }
                else
                {
                    string insertsql = "insert into DataAuthority (userid,depid) values ('" + userid.Trim() + "','" + depid.Trim() + "');";
                    ExecSql(insertsql.ToString());
                }
                HttpContext.Current.Response.Write("授权成功！");
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }
        public static Boolean ExecSql(string sql)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionStringByConfig()))
            {
                conn.Open();
                SqlTransaction sqlTran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Transaction = sqlTran;
                    cmd.ExecuteNonQuery();//专门执行修改的方法，返回整数
                    sqlTran.Commit();
                    conn.Close();
                    return true;
                }
                catch (Exception e)
                {
                    sqlTran.Rollback();
                    conn.Close();
                    return false;
                }
            }
        }
        public static string GetConnectionStringByConfig()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
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