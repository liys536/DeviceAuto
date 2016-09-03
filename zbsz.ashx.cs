using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DeviceAuto
{
    /// <summary>
    /// zbsz 的摘要说明
    /// </summary>
    public class zbsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string id = context.Request["zid"];
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

                string strWhere = GetWhere();
                if (!string.IsNullOrEmpty(id))
                {
                    strWhere = strWhere + " and id=" + id;
                }

                DataSet duser = SqlHelper.GetList("v_zbsz", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_zbsz where " + strWhere);
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

                //板信息
                sql = "delete from bszb where izid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //站信息
                sql = "delete from zszb where id=" + id;
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
                string[] str = ss.Split('~');
                string[] strT = str[0].Split('&');     //站信息

                DataTable dt = new DataTable();

                //查找id最大值
                Int64 id = 0;
                dt = SqlHelper.GetTable("select max(id) as maxid from zszb");
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

                //查找id最大值
                Int64 bid = 0;
                dt = SqlHelper.GetTable("select max(id) as maxid from bszb");
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["maxid"].ToString()))
                    {
                        bid = Int64.Parse(dt.Rows[0]["maxid"].ToString()) + 1;
                    }
                    else
                    {
                        bid = 1;
                    }
                }
                else
                {
                    bid = 1;
                }
                dt = null;


                string czdz = strT[1].Split('=')[1];  //站编号

                if (czdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                dt = SqlHelper.GetTable("select * from zszb where czdz='" + czdz + "'");
                if (dt.Rows.Count > 0)
                {
                    dt = null;
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                dt = null;

                string czmc = strT[2].Split('=')[1];  //站名称

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@zdz",czdz),
                            new SqlParameter("@zmc",czmc)
                                 };
                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "insert_zsz";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //插入板
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strB = str[1].Split('*')[1].Split('@');
                string[] strr;
                string cbdz = "";
                string cbmc = "";
                string sql = "";


                for (int i = 0; i < total; i++)
                {
                    strr = strB[i].Split('&');
                    cbdz = strr[1];

                    if (cbdz.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    cbmc = strr[2];

                    if (cbmc.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("5&" + (i + 1));
                        return;
                    }

                    //判断板地址是否存在
                    sql = "select * from bszb where cbdz='" + cbdz + "' and izid=" + id;
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = sql;
                    comm.CommandType = CommandType.Text;
                    object srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("6&" + (i + 1));
                        return;
                    }

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@id",bid + i),
                            new SqlParameter("@zid",id),
                            new SqlParameter("@bdz",cbdz),
                            new SqlParameter("@bmc",cbmc)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_bsz";
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
                string[] strT = str[0].Split('&');     //站信息
                int id = int.Parse(strT[0].Split('=')[1]);
                string czdz = strT[1].Split('=')[1];  //站地址

                //查找id最大值
                Int64 bid = 0;
                DataTable  dt = SqlHelper.GetTable("select max(id) as maxid from bszb");
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["maxid"].ToString()))
                    {
                        bid = Int64.Parse(dt.Rows[0]["maxid"].ToString()) + 1;
                    }
                    else
                    {
                        bid = 1;
                    }
                }
                else
                {
                    bid = 1;
                }
                dt = null;

                if (czdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                dt = SqlHelper.GetTable("select * from zszb where czdz='" + czdz + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    dt = null;
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                dt = null;

                string czmc = strT[2].Split('=')[1];  //站名称

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@zdz",czdz),
                            new SqlParameter("@zmc",czmc)
                                 };
                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "update_zsz";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //删除板信息
                SqlParameter[] parm = {
                            new SqlParameter("@zid",id)
                                 };
                SqlCommand comd = new SqlCommand();
                comd.Connection = con;//获取数据连接
                comd.Transaction = sqltra;//，在执行SQL时
                comd.CommandText = "update_bsz";
                comd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm)
                {
                    comd.Parameters.Add(p);
                }
                comd.ExecuteNonQuery();

                //插入板
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strB = str[1].Split('*')[1].Split('@');
                string[] strr;
                string cbdz = "";
                string cbmc = "";
                string sql = "";

                for (int i = 0; i < total; i++)
                {
                    strr = strB[i].Split('&');
                    cbdz = strr[1];

                    if (cbdz.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    cbmc = strr[2];

                    if (cbmc.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("5&" + (i + 1));
                        return;
                    }

                    //判断板编号是否存在
                    sql = "select * from bszb where cbdz='" + cbdz + "' and izid=" + id;
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = sql;
                    comm.CommandType = CommandType.Text;
                    object srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("6&" + (i + 1));
                        return;
                    }

                    SqlParameter[] parms_mx = {
                             new SqlParameter("@id",bid + i),
                            new SqlParameter("@zid",id),
                            new SqlParameter("@bdz",cbdz),
                            new SqlParameter("@bmc",cbmc)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_bsz";
                    coms.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        coms.Parameters.Add(p);
                    }
                    coms.ExecuteNonQuery();
                }

                //删除板信息中的冗余信息
                sql = "delete from tdszb where not exists (select ibid from bszb where izid=tdszb.izid and cbdz=tdszb.cbdz)";
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

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