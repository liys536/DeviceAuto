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
    /// tjjl 的摘要说明
    /// </summary>
    public class tjjl : IHttpHandler
    {
        string state = HttpContext.Current.Request["state"];
        string sbid = HttpContext.Current.Request["sb"];

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "query":
                    Query(sbid, state);
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
        /// 查询的方法
        /// </summary>
        private void Query(string csbid,string sState)
        {
            try
            {
                if (sState == "" || string.IsNullOrEmpty(sState))
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "1=1";

                if (!string.IsNullOrEmpty(sState) && sState == "s")
                {
                    if (!string.IsNullOrEmpty(csbid))
                    {
                        strWhere = strWhere + " and isbid=" + csbid;
                    }
                }
                else
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                string qsrq = HttpContext.Current.Request["qsrq"];
                if (!string.IsNullOrEmpty(qsrq))
                {
                    strWhere = strWhere + " and dtxrq>='" + qsrq + "'";
                }

                string jzrq = HttpContext.Current.Request["jzrq"];
                if (!string.IsNullOrEmpty(jzrq))
                {
                    strWhere = strWhere + " and dtxrq<='" + jzrq + "'";
                }

                string ckey = HttpContext.Current.Request["ckey"];
                string ctext = HttpContext.Current.Request["cmemo"];

                if (!string.IsNullOrEmpty(ckey))
                {
                    switch (ckey)
                    {
                        case "单号":
                            strWhere = strWhere + " and cdh like '%" + ctext + "%'";
                            break;
                        case "设备编号":
                            strWhere = strWhere + " and csbbh like '%" + ctext + "%'";
                            break;
                        case "设备名称":
                            strWhere = strWhere + " and csbmc like '%" + ctext + "%'";
                            break;
                        case "生产线":
                            strWhere = strWhere + " and cscx like '%" + ctext + "%'";
                            break;
                        case "停机种类":
                            strWhere = strWhere + " and ctjzl like '%" + ctext + "%'";
                            break;
                        case "现象描述":
                            strWhere = strWhere + " and cxxms like '%" + ctext + "%'";
                            break;
                        case "解决方案":
                            strWhere = strWhere + " and cjjfa like '%" + ctext + "%'";
                            break;
                        case "原因分析":
                            strWhere = strWhere + " and cyyfx like '%" + ctext + "%'";
                            break;
                        case "改善对策":
                            strWhere = strWhere + " and cgsdc like '%" + ctext + "%'";
                            break;
                        case "作业人":
                            strWhere = strWhere + " and ctjr like '%" + ctext + "%'";
                            break;
                        case "制单人":
                            strWhere = strWhere + " and cczy like '%" + ctext + "%'";
                            break;
                        case "辅助人员":
                            strWhere = strWhere + " and cfzry like '%" + ctext + "%'";
                            break;
                        case "备注":
                            strWhere = strWhere + " and cbz like '%" + ctext + "%'";
                            break;
                    }

                }


                DataSet duser = SqlHelper.GetList("v_tjjl", "*", "id", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_tjjl where " + strWhere + " order by dtxrq desc");
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
                count = SqlHelper.DelData("tjjlb", id);
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
                string[] str = ss.Split('&');

                DataTable dt = new DataTable();

                //查找id最大值
                Int64 id = 0;
                dt = SqlHelper.GetTable("select max(id) as maxid from tjjlb");
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

                string cdh = str[1].Split('=')[1];
                if (string.IsNullOrEmpty(cdh) || cdh == "")
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                dt = SqlHelper.GetTable("select * from tjjlb where cdh='" + cdh + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                sys ex = new sys();
                string drq = "";
                if (ex.isDate(str[2].Split('=')[1]) == true)
                {
                    drq = str[2].Split('=')[1];
                }

                string csbbh = str[3].Split('=')[1];

                if (string.IsNullOrEmpty(csbbh) || csbbh == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string isbid = "";
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    //设备编号不存在
                    HttpContext.Current.Response.Write("4");
                    return;
                }
                else
                {
                    isbid = dt.Rows[0]["id"].ToString();
                }

                string dkjrq = "";
                if (ex.isDate(str[7].Split('=')[1]) == true)
                {
                    dkjrq = str[7].Split('=')[1].Replace("+", " "); 
                }
                string dtjrq = "";
                if (ex.isDate(str[8].Split('=')[1]) == true)
                {
                    dtjrq = str[8].Split('=')[1].Replace("+", " "); 
                }
                string ctjzl = str[9].Split('=')[1];
                string cxxms = str[10].Split('=')[1];
                string cjjfa = str[11].Split('=')[1];
                string cyyfx = str[12].Split('=')[1];
                string cgsdc = str[13].Split('=')[1];
                string ctjr = str[14].Split('=')[1];
                //if (string.IsNullOrEmpty(ctjr) || ctjr.Trim() == "")
                //{
                //    HttpContext.Current.Response.Write("9");
                //    return;
                //}

                string cfzry = str[15].Split('=')[1];
                string cczy = str[16].Split('=')[1];
                string cbz = str[17].Split('=')[1];

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter ("@id",id),
                            new SqlParameter("@dh",cdh),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@tjzl",ctjzl),
                            new SqlParameter("@kjrq",dkjrq),
                            new SqlParameter("@tjrq",dtjrq),
                            new SqlParameter("@xxms",cxxms),
                            new SqlParameter("@jjfa",cjjfa),
                            new SqlParameter("@yyfx",cyyfx),
                            new SqlParameter("@gsdc",cgsdc),
                            new SqlParameter("@tjr",ctjr),
                            new SqlParameter("@fzry",cfzry),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz)
                                 };

                cmd.CommandText = "insert_tjjl";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();


                //履历表
                SqlParameter[] parm_llb = {
                            new SqlParameter("@xmid",id),
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@zysj",drq),
                            new SqlParameter("@zyxm","停机"),
                            new SqlParameter("@zyry",ctjr),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@zynr",cxxms)
                                 };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "insert_llb";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm_llb)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

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
                string cdh = str[1].Split('=')[1];
                if (string.IsNullOrEmpty(cdh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from tjjlb where cdh='" + cdh + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                sys ex = new sys();
                string drq = "";
                if (ex.isDate(str[2].Split('=')[1]) == true)
                {
                    drq = str[2].Split('=')[1];
                }

                string csbbh = str[3].Split('=')[1];

                if (string.IsNullOrEmpty(csbbh) || csbbh == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string isbid = "";
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    //设备编号不存在
                    HttpContext.Current.Response.Write("4");
                    return;
                }
                else
                {
                    isbid = dt.Rows[0]["id"].ToString();
                }

                string dkjrq = "";
                if (ex.isDate(str[7].Split('=')[1]) == true)
                {
                    dkjrq = str[7].Split('=')[1].Replace("+", " "); 
                }
                string dtjrq = "";
                if (ex.isDate(str[8].Split('=')[1]) == true)
                {
                    dtjrq = str[8].Split('=')[1].Replace("+", " "); 
                }
                string ctjzl = str[9].Split('=')[1];
                string cxxms = str[10].Split('=')[1];
                string cjjfa = str[11].Split('=')[1];
                string cyyfx = str[12].Split('=')[1];
                string cgsdc = str[13].Split('=')[1];
                string ctjr = str[14].Split('=')[1];
                //if (string.IsNullOrEmpty(ctjr) || ctjr.Trim() == "")
                //{
                //    HttpContext.Current.Response.Write("9");
                //    return;
                //}

                string cfzry = str[15].Split('=')[1];
                string cczy = str[16].Split('=')[1];
                string cbz = str[17].Split('=')[1];

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@dh",cdh),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@tjzl",ctjzl),
                            new SqlParameter("@kjrq",dkjrq),
                            new SqlParameter("@tjrq",dtjrq),
                            new SqlParameter("@xxms",cxxms),
                            new SqlParameter("@jjfa",cjjfa),
                            new SqlParameter("@yyfx",cyyfx),
                            new SqlParameter("@gsdc",cgsdc),
                            new SqlParameter("@tjr",ctjr),
                            new SqlParameter("@fzry",cfzry),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@id",id)
                                 };
                cmd.CommandText = "update_tjjl";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parms)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();


                //履历表
                SqlParameter[] parm_llb = {
                            new SqlParameter("@xmid",id),
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@zysj",drq),
                            new SqlParameter("@zyxm","停机"),
                            new SqlParameter("@zyry",ctjr),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@zynr",cxxms)
                                 };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "update_llb";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm_llb)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

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