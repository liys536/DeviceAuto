using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Configuration;

namespace DeviceAuto
{
    /// <summary>
    /// gcstz 的摘要说明
    /// </summary>
    public class gcstz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string username = context.Request["data"];
            string state = context.Request["state"];
            switch (action)
            {
                case "query":
                    Query();
                    break;
                case "del":
                    Del();
                    break;
                case "add":
                    Add();
                    break;
                case "edit":
                    Edit();
                    break;
                case "export":
                    Export();
                    break;
                case "search":
                    Search(username);
                    break;
                case "show":
                    Show(username,state);
                    break;
            }
        }

        //个人通知
        private void Show(string user,string sState)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "1=1";

                if (!string.IsNullOrEmpty(sState) && sState == "s")
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        strWhere = strWhere + " and cdxmc='" + user + "'";
                    }
                }
                else
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                DataSet duser = SqlHelper.GetList("tmp_gcstzb", "*", "id", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from tmp_gcstzb where " + strWhere + " order by drq desc");
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

        /// <summary>
        /// 通知数
        /// </summary>
        private void Search(string user)
        {
            Int64 iCount = 0;
            DataTable dt = SqlHelper.GetTable("select count(*) as n from tmp_gcstzb where cdxmc='" + user + "'");
            iCount = Int64.Parse(dt.Rows[0]["n"].ToString());
            HttpContext.Current.Response.Write("操作员 " + user + "," + "您有" + iCount + "条通知!");
        }

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "通知" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data = HttpContext.Current.Request["data"];
            File.WriteAllText(HttpContext.Current.Server.MapPath(fn), data, Encoding.UTF8);//如果是gb2312的xml申明，第三个编码参数修改为Encoding.GetEncoding(936)

            HttpContext.Current.Response.Write(fn);//返回文件名提供下载
        }


        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query()
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string state = HttpContext.Current.Request["state"];
                if (string.IsNullOrEmpty(state))
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                string strWhere ="1=1";

                string qsrq = HttpContext.Current.Request["qsrq"];
                if (!string.IsNullOrEmpty(qsrq))
                {
                    strWhere = strWhere + " and drq>='" + qsrq + "'";
                }

                string jzrq = HttpContext.Current.Request["jzrq"];
                if (!string.IsNullOrEmpty(jzrq))
                {
                    strWhere = strWhere + " and drq<='" + jzrq + "'";
                }

                string ckey = HttpContext.Current.Request["ckey"];
                string ctext = HttpContext.Current.Request["cmemo"];

                if (!string.IsNullOrEmpty(ckey))
                {
                    switch (ckey)
                    {
                        case "对象":
                            strWhere = strWhere + " and cdx like '%" + ctext + "%'";
                            break;
                        case "对象名称":
                            strWhere = strWhere + " and cdxmc like '%" + ctext + "%'";
                            break;
                        case "记录":
                            strWhere = strWhere + " and cjl like '%" + ctext + "%'";
                            break;
                        case "制单人":
                            strWhere = strWhere + " and cczy like '%" + ctext + "%'";
                            break;
                    }

                }

                DataSet duser = SqlHelper.GetList("v_gcstz", "*", "id", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_gcstz where " + strWhere + " order by drq desc");
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

        //删除的方法
        private void Del()
        {
            try
            {
                //获取到选中行的id
                string id = HttpContext.Current.Request["id"];

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                string sql = "";

                //工程师通知
                sql = "delete from gcstzb where id=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //临时工程师通知
                sql = "delete from tmp_gcstzb where id=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();


                sqltra.Commit();        //提交事务

                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("删除成功！");
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }
        //添加
        private void Add()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //遍历获取传递过来的字符串
                foreach (string s in HttpContext.Current.Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
                }
                string ss = sb.ToString();
                string[] str = ss.Split('&');
                sys ex = new sys();
                string drq = "";

                //查找id最大值
                Int64 id = 0;
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select max(id) as maxid from gcstzb");
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["maxid"].ToString()))
                    {
                        id = Int64.Parse(dt.Rows[0]["maxid"].ToString()) + 1;
                    }
                    else
                    {
                        id = 1;
                    }
                }
                else
                {
                    id = 1;
                }
                dt = null;

                if (ex.isDate(str[1].Split('=')[1]) == true)
                {
                    drq = str[1].Split('=')[1];
                }
                string dx = str[2].Split('=')[1];
                string cdxmc = "";
                if (str[3].Split('=')[1].Trim().Length > 0)
                {
                    cdxmc = str[3].Split('=')[1];
                }
                if (str[4].Split('=')[1].Trim().Length > 0)
                {
                    cdxmc = str[4].Split('=')[1];
                }
                string cjl = str[5].Split('=')[1];
                string cczy = str[6].Split('=')[1];

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parm= {
                            new SqlParameter("@id",id),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@dx",dx),
                            new SqlParameter("@dxmc",cdxmc),
                            new SqlParameter("@jl",cjl),
                            new SqlParameter("@czy",cczy)
                                 };
  
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insert_gcstz";
                foreach (SqlParameter p in parm)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //临时表
                string[] strr = cdxmc.Split(',');
                int len = strr.Length;
                string value = "";

                if (len > 0)
                {
                    //员工分割
                    for (int i = 0; i < len; i++)
                    {
                        value = strr[i].Split('-')[0];

                        SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@dx",dx),
                            new SqlParameter("@dxmc",value),
                            new SqlParameter("@jl",cjl),
                            new SqlParameter("@czy",cczy)
                                 };

                        SqlCommand cmds = new SqlCommand();
                        cmds.Connection = con;//获取数据连接
                        cmds.Transaction = sqltra;//，在执行SQL时
                        cmds.CommandType = CommandType.StoredProcedure;
                        cmds.CommandText = "insert_tmp_gcstz";
                        foreach (SqlParameter p in parms)
                        {
                            cmds.Parameters.Add(p);
                        }
                        cmds.ExecuteNonQuery();
                        cmds.Dispose();
                    }
                }

                sqltra.Commit();        //提交事务

                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("添加成功！");


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
                StringBuilder sb = new StringBuilder();
                //遍历获取传递过来的字符串
                foreach (string s in HttpContext.Current.Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
                }
                string ss = sb.ToString();
                string[] str = ss.Split('&');
                int id = int.Parse(str[0].Split('=')[1]);
                sys ex = new sys();
                string drq = "";
                if (ex.isDate(str[1].Split('=')[1]) == true)
                {
                    drq = str[1].Split('=')[1];
                }
                string dx = str[2].Split('=')[1];
                string cdxmc = "";
                if (str[3].Split('=')[1].Trim().Length > 0)
                {
                    cdxmc = str[3].Split('=')[1];
                }
                if (str[4].Split('=')[1].Trim().Length > 0)
                {
                    cdxmc = str[4].Split('=')[1];
                }
                string cjl = str[5].Split('=')[1];
                string cczy = str[6].Split('=')[1];


                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parm = {
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@dx",dx),
                            new SqlParameter("@dxmc",cdxmc),
                            new SqlParameter("@jl",cjl),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@id",id)
                                 };

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "update_gcstz";
                foreach (SqlParameter p in parm)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //临时工程师通知
                string sql = "delete from tmp_gcstzb where id=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //临时表
                string[] strr = cdxmc.Split(',');
                int len = strr.Length;
                string value = "";

                if (len > 0)
                {
                    //员工分割
                    for (int i = 0; i < len; i++)
                    {
                        value = strr[i].Split('-')[0];

                        SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@dx",dx),
                            new SqlParameter("@dxmc",value),
                            new SqlParameter("@jl",cjl),
                            new SqlParameter("@czy",cczy)
                                 };

                        SqlCommand cmds = new SqlCommand();
                        cmds.Connection = con;//获取数据连接
                        cmds.Transaction = sqltra;//，在执行SQL时
                        cmds.CommandType = CommandType.StoredProcedure;
                        cmds.CommandText = "insert_tmp_gcstz";
                        foreach (SqlParameter p in parms)
                        {
                            cmds.Parameters.Add(p);
                        }
                        cmds.ExecuteNonQuery();
                        cmds.Dispose();
                    }
                }

                sqltra.Commit();        //提交事务

                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("修改成功！");
                
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