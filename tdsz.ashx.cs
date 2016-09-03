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
    /// tdsz 的摘要说明
    /// </summary>
    public class tdsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string bid=context.Request["bid"];
            switch (action)
            {
                case "query":
                    Query(bid);
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
                case "select":
                    Select();
                    break;
            }
        }

        /// <summary>
        /// 选择站板名称
        /// </summary>
        private void Select()
        {
            try
            {
                string bid=HttpContext.Current.Request["data"];
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from v_zbsz where bid=" + bid);
                if (dt.Rows.Count > 0)
                {
                    string cData = dt.Rows[0]["czdz"] + "&" + dt.Rows[0]["czmc"] + "*"  +dt.Rows[0]["cbdz"] + "," + dt.Rows[0]["cbmc"];
                    HttpContext.Current.Response.Write(cData);
                }
                else
                {
                    HttpContext.Current.Response.Write("null");
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
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
                    strWhere = " ibid=" + id;
                }

                DataSet duser = SqlHelper.GetList("v_tdsz", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_tdsz where " + strWhere);
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
                int count = 0;
                count = SqlHelper.DelData("tdszb", id);
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
                string[] strT = str[0].Split('&');     //站信息

                DataTable dt = new DataTable();

                int bid = int.Parse(strT[1].Split('=')[1]);

                string czdz = strT[2].Split('=')[1];  //站地址
                if (czdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                string cbdz = strT[4].Split('=')[1];     //板地址
                if (cbdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }


                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                //插入通道信息
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strB = str[1].Split('*')[1].Split('@');
                string[] strr;
                string ctddz = "";
                string cblm = "";
                string cdw = "";
                double ixx = 0;
                double isx = 0;
                double idyxx = 0;
                double idysx = 0;
                double ipd = 0;
                string cfz = "";
                string sql = "";

                for (int i = 0; i < total; i++)
                {
                    strr = strB[i].Split('&');
                    ctddz = strr[1];

                    if (ctddz.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("3&" + (i + 1));
                        return;
                    }

                    //判断板通道名称是否存在
                    sql = "select * from tdszb where ctddz='" + ctddz + "' and ibid=" + bid;
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

                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    cblm = strr[2];

                    if (cblm.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("5&" + (i + 1));
                        return;
                    }

                    //判断变量名是否存在
                    sql = "select * from tdszb where cblm='" + cblm + "'";
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = sql;
                    comm.CommandType = CommandType.Text;
                    srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("6&" + (i + 1));
                        return;
                    }

                    cdw = strr[3];
                    sys s = new sys();
                    if (s.isNumber(strr[4]) == true) ixx = double.Parse(strr[4]);
                    if (s.isNumber(strr[5]) == true) idyxx = double.Parse(strr[5]);
                    if (s.isNumber(strr[6]) == true) isx = double.Parse(strr[6]);
                    if (s.isNumber(strr[7]) == true) idysx = double.Parse(strr[7]);
                    if (s.isNumber(strr[8]) == true) ipd = double.Parse(strr[8]);
                    cfz = strr[9];

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@bid",bid),
                            new SqlParameter("@zdz",czdz),
                            new SqlParameter("@bdz",cbdz),
                            new SqlParameter("@tddz",ctddz),
                            new SqlParameter("@blm",cblm),
                            new SqlParameter("@dw",cdw),
                            new SqlParameter("@xx",ixx),
                            new SqlParameter("@dyxx",idyxx),
                            new SqlParameter("@sx",isx),
                            new SqlParameter("@dysx",idysx),
                            new SqlParameter("@pd",ipd),
                            new SqlParameter("@fz",cfz)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_tdsz";
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
                string[] strT = str[0].Split('&');     //板信息
                int yid = int.Parse(strT[0].Split('=')[1]);
                int bid = int.Parse(strT[1].Split('=')[1]);

                string czdz = strT[2].Split('=')[1];  //站地址
                if (czdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                string cbdz = strT[4].Split('=')[1];     //板地址
                if (cbdz.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }


                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@bid",yid)
                                 };
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时
                cmd.CommandText = "update_tdsz";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();

                //插入通道信息
                int total = int.Parse(str[1].Split('*')[2]);
                string[] strB = str[1].Split('*')[1].Split('@');
                string[] strr;
                string ctddz = "";
                string cblm = "";
                string cdw = "";
                double ixx = 0;
                double isx = 0;
                double idyxx = 0;
                double idysx = 0;
                double ipd = 0;
                string cfz = "";
                string sql = "";

                for (int i = 0; i < total; i++)
                {
                    strr = strB[i].Split('&');
                    ctddz = strr[1];

                    if (ctddz.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("3&" + (i + 1));
                        return;
                    }

                    //判断板通道名称是否存在
                    sql = "select * from tdszb where ctddz='" + ctddz + "' and ibid=" + bid;
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

                        HttpContext.Current.Response.Write("4&" + (i + 1));
                        return;
                    }

                    cblm = strr[2];

                    if (cblm.Trim().Length == 0)
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("5&" + (i + 1));
                        return;
                    }

                    //判断变量名是否存在
                    sql = "select * from tdszb where cblm='" + cblm + "'";
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = sql;
                    comm.CommandType = CommandType.Text;
                    srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("6&" + (i + 1));
                        return;
                    }

                    cdw = strr[3];
                    sys s = new sys();
                    if (s.isNumber(strr[4]) == true) ixx = double.Parse(strr[4]);
                    if (s.isNumber(strr[5]) == true) idyxx = double.Parse(strr[5]);
                    if (s.isNumber(strr[6]) == true) isx = double.Parse(strr[6]);
                    if (s.isNumber(strr[7]) == true) idysx = double.Parse(strr[7]);
                    if (s.isNumber(strr[8]) == true) ipd = double.Parse(strr[8]);
                    cfz = strr[9];

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@bid",bid),
                            new SqlParameter("@zdz",czdz),
                            new SqlParameter("@bdz",cbdz),
                            new SqlParameter("@tddz",ctddz),
                            new SqlParameter("@blm",cblm),
                            new SqlParameter("@dw",cdw),
                            new SqlParameter("@xx",ixx),
                            new SqlParameter("@dyxx",idyxx),
                            new SqlParameter("@sx",isx),
                            new SqlParameter("@dysx",idysx),
                            new SqlParameter("@pd",ipd),
                            new SqlParameter("@fz",cfz)
                                 };

                    SqlCommand coms = new SqlCommand();
                    coms.Connection = con;//获取数据连接
                    coms.Transaction = sqltra;//，在执行SQL时
                    coms.CommandText = "insert_tdsz";
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