using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;


namespace DeviceAuto
{
    /// <summary>
    /// djfs_plsz 的摘要说明
    /// </summary>
    public class djfs_plsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];

            switch (action)
            {
                case "batch":
                    Batch();
                    break;
            }
        }

        public void Batch()
        {
            try
            {
                string str = HttpContext.Current.Request["data"];
                string[] strr = str.Split('@');
                int rows = int.Parse(strr[1]);
                int djid = int.Parse(strr[2]);
                string ss = "";
                string sql = "";
                string ssql = "";


                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                string csbbh = "";
                string csbmc = "";

                for (int i = 0; i <= rows; i++)
                {
                    ss = strr[0].Split('&')[i + 1];       //设备id

                    //sql = "select * from sbzlb_djfs where idjid=" + djid + " and isbid=" + ss;
                    //SqlCommand comm = new SqlCommand();
                    //comm.Connection = con;
                    //comm.Transaction = sqltra;//，在执行SQL时
                    //comm.CommandText = sql;
                    //comm.CommandType = CommandType.Text;
                    //object srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    //if (srows != null)
                    //{
                    //    comm.Dispose();
                    //    con.Close();
                    //    con.Dispose();

                    //    HttpContext.Current.Response.Write("A&" + ss);
                    //    return;
                    //}


                    sql = "select * from v_sbzl_dj where idjid=" + djid + " and isbid=" + ss;
                    SqlDataAdapter sd = new SqlDataAdapter(sql, con);
                    SqlCommandBuilder scb = new SqlCommandBuilder(sd);
                    sd.SelectCommand.Transaction = sqltra;
                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        csbbh = dt.Rows[0]["DeviceId"].ToString();
                        csbmc = dt.Rows[0]["devicename"].ToString();

                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("A&" + csbbh + "," + csbmc );
                        return;
                    }
                    

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",ss),
                            new SqlParameter("@djid",djid),
                            new SqlParameter("@djdw","小时"),
                            new SqlParameter("@djzq","0"),
                            new SqlParameter("@djr",""),
                            new SqlParameter("@bjr","")
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbdj_pl";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();



                }

                sqltra.Commit();        //提交事务

                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("批量设备点检方式成功！");
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