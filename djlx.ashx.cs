using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration ;

namespace DeviceAuto
{
    /// <summary>
    /// djlx 的摘要说明
    /// </summary>
    public class djlx : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action=context.Request["action"];
            string id = context.Request["sb"];
            switch (action)
            {
                case "query":
                    Query(id);
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
            }
        }

        /// <summary>
        /// 组合搜索条件
        /// </summary>
        /// <returns></returns>
        private string GetWhere()
        {
            StringBuilder sb = new StringBuilder("1=1");
            string searchType = HttpContext.Current.Request["search_type"] != "" ? HttpContext.Current.Request["search_type"] : string.Empty;
            string searchValue = HttpContext.Current.Request["search_value"] != "" ? HttpContext.Current.Request["search_value"] : string.Empty;
            if (searchType != null && searchValue != null)
            {
                sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string id)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "";
                if (string.IsNullOrEmpty(id) || id == "0")
                {
                    strWhere = GetWhere();
                }
                else
                {
                    strWhere = " id=" + id;
                }

                DataSet duser = SqlHelper.GetList("v_djlx", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_djlx where " + strWhere);
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

                string sql = "delete from djlxb_mx where ilxid=" + id;
                SqlHelper.TransactSql(sql);

                int count = 0;
                count = SqlHelper.DelData("djlxb", id);
                if (count > 0)
                {
                    HttpContext.Current.Response.Write("共删除了" + count + "条数据");
                }
                else
                {
                    HttpContext.Current.Response.Write("error");
                }
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
                string[] str = ss.Split('~');
                string[] strT = str[0].Split('&');     //设备资料

                DataTable dt = new DataTable();

                //查找id最大值
                Int64 id = 0;
                dt = SqlHelper.GetTable("select max(id) as maxid from djlxb");
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

                string clxmc = strT[1].Split('=')[1];  //路线名称

                if (clxmc.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                dt = SqlHelper.GetTable("select * from djlxb where cdjlx='" + clxmc + "'");
                if (dt.Rows.Count > 0)
                {
                    dt = null;
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                dt = null;

                sys ex = new sys();
                string drq = "";
                if (ex.isDate((strT[2].Split('=')[1])) == true)
                {
                    drq = strT[2].Split('=')[1];
                }

                string csjz = strT[3].Split('=')[1];     //设计者

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@djlx",clxmc),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@sjz",csjz)
                                 };
                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "insert_djlx";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //插入明细
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strSb = str[1].Split('*')[1].Split('@');
                string[] strr;
                string csbbh = "";
                Int64 ixh = 0;
                Int64 isbid = 0;
                string sql = "";

                for (int i = 0; i < total; i++)
                {
                    strr = strSb[i].Split('&');
                    if (ex.isNumber(strr[1]) == true)
                    {
                        ixh = Int64.Parse(strr[1]);
                    }
                    isbid = Int64.Parse(strr[2]);

                    csbbh = strr[3];

                    //不合适的序号
                    if (ixh != i + 1)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("3&" + (i + 1));
                        return;
                    }

                    if (string.IsNullOrEmpty(csbbh))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    ////判断部件编号是否存在
                    //sql = "select * from sbzlb where DeviceId='" + csbbh + "'";
                    //dt = SqlHelper.GetTable(sql);
                    //if (dt.Rows.Count == 0)
                    //{
                    //    dt = null;      //出错后释放

                    //    con.Close();
                    //    HttpContext.Current.Response.Write("5&" + (i + 1));
                    //    return;
                    //}
                    //dt = null;

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@lxid",id),
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@xh",ixh)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_djlx_mx";
                    coms.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        coms.Parameters.Add(p);
                    }
                    coms.ExecuteNonQuery();

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
                string[] str = ss.Split('~');
                string[] strT = str[0].Split('&');     //设备资料
                int id = int.Parse(strT[0].Split('=')[1]);
                string clxmc = strT[1].Split('=')[1];

                if (string.IsNullOrEmpty(clxmc))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from djlxb where cdjlx='" + clxmc + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                sys ex = new sys();
                string drq = "";
                if (ex.isDate((strT[2].Split('=')[1])) == true)
                {
                    drq = strT[2].Split('=')[1];
                }

                string csjz = strT[3].Split('=')[1];     //设计者

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@djlx",clxmc),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@sjz",csjz)
                                 };
                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "update_djlx";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                SqlParameter[] parm = {
                            new SqlParameter("@lxid",id)
                                 };
                SqlCommand comd = new SqlCommand();
                comd.Connection = con;//获取数据连接
                comd.Transaction = sqltra;//，在执行SQL时
                comd.CommandText = "update_djlx_mx";
                comd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm)
                {
                    comd.Parameters.Add(p);
                }
                comd.ExecuteNonQuery();

                //插入明细
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strSb = str[1].Split('*')[1].Split('@');
                string[] strr;
                string csbbh = "";
                Int64 ixh = 0;
                Int64 isbid = 0;
                string sql = "";

                for (int i = 0; i < total; i++)
                {
                    strr = strSb[i].Split('&');
                    if (ex.isNumber(strr[1]) == true)
                    {
                        ixh = Int64.Parse(strr[1]);
                    }
                    isbid = Int64.Parse(strr[2]);

                    csbbh = strr[3];

                    //不合适的序号
                    if (ixh != i + 1)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("3&" + (i + 1));
                        return;
                    }

                    if (string.IsNullOrEmpty(csbbh))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    ////判断部件编号是否存在
                    //sql = "select * from sbzlb where DeviceId='" + csbbh + "'";
                    //dt = SqlHelper.GetTable(sql);
                    //if (dt.Rows.Count == 0)
                    //{
                    //    dt = null;      //出错后释放

                    //    con.Close();
                    //    HttpContext.Current.Response.Write("5&" + (i + 1));
                    //    return;
                    //}
                    //dt = null;

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@lxid",id),
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@xh",ixh)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_djlx_mx";
                    coms.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        coms.Parameters.Add(p);
                    }
                    coms.ExecuteNonQuery();

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