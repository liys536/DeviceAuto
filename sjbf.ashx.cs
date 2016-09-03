using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// sjbf 的摘要说明
    /// </summary>
    public class sjbf : IHttpHandler
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
                case "backup":
                    Backup();
                    break;
            }
        }

        public void Query()
        {
            string bakpath = ConfigurationManager.ConnectionStrings["bakpath"].ConnectionString;
            HttpContext.Current.Response.Write(bakpath);
        }

        public void Backup()
        {
            try
            {
                //根据连接字符串获取数据库名称
                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                string db = strConn.Split(';')[1];
                string dbname = db.Split('=')[1];
                string bkpath = HttpContext.Current.Request["data"];       //使用单个符号“\”提示常量中有换行符

                //自动创建文件夹
                FileInfo fi = new FileInfo(bkpath);
                var di = fi.Directory;
                if (!di.Exists) di.Create();

                SqlConnection con = new SqlConnection(SqlHelper.conString);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "exec p_backupdb '" + dbname + "','" + bkpath + "'";
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("数据备份成功！");
            }

            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog (ex);
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