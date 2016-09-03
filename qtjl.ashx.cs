using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text ;
using System.Configuration;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// qtjl 的摘要说明
    /// </summary>
    public class qtjl : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string state = context.Request["state"];
            string sbid=context.Request["sb"];
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
                //case "add2":
                //    Add2();
                //    break;
                case "edit":
                    Edit();
                    break;
                //case "edit2":
                //    Edit2();
                //    break;
                case "export":
                    Export();
                    break;
            }
        }

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "异常记录" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data = HttpContext.Current.Request["data"];
            File.WriteAllText(HttpContext.Current.Server.MapPath(fn), data, Encoding.UTF8);//如果是gb2312的xml申明，第三个编码参数修改为Encoding.GetEncoding(936)

            HttpContext.Current.Response.Write(fn);//返回文件名提供下载
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

                string strWhere ="1=1";

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
                        case "工单号":
                            strWhere = strWhere + " and cgdh like '%" + ctext + "%'";
                            break;
                        case "设备名称":
                            strWhere = strWhere + " and csbmc like '%" + ctext + "%'";
                            break;
                        case "生产线":
                            strWhere = strWhere + " and cscx like '%" + ctext + "%'";
                            break;
                        case "专业":
                            strWhere = strWhere + " and czy like '%" + ctext + "%'";
                            break;
                        case "异常描述":
                            strWhere = strWhere + " and cycms like '%" + ctext + "%'";
                            break;
                        case "处理记录":
                            strWhere = strWhere + " and ccljl like '%" + ctext + "%'";
                            break;
                        case "作业人":
                            strWhere = strWhere + " and czyr like '%" + ctext + "%'";
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

                DataSet duser = SqlHelper.GetList("v_ycjl", "*", "id", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_ycjl where " + strWhere + " order by drq desc");
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

                //异常记录明细
                sql = "delete from ycjlb_mx where iycid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //异常记录主表
                sql = "delete from ycjlb where id=" + id;
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
                string[] strSb = str[0].Split('&');     //设备资料

                DataTable dt = new DataTable();

                //查找id最大值
                Int64 id = 0;
                dt = SqlHelper.GetTable("select max(id) as maxid from ycjlb");
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

                string cgdh = strSb[1].Split('=')[1];  //工单号

                dt = SqlHelper.GetTable("select * from ycjlb where cgdh='" + cgdh + "'");
                if (dt.Rows.Count > 0)
                {
                    dt = null;
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                dt = null;

                sys ex = new sys();
                string drq = "";
                if (ex.isDate((strSb[2].Split('=')[1])) == true)
                {
                    drq = strSb[2].Split('=')[1];
                }

                string cczy = strSb[3].Split('=')[1];  //操作员

                string csbmc = strSb[4].Split('=')[1];  //设备名称

                if (string.IsNullOrEmpty(csbmc))
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cscx = strSb[5].Split('=')[1];  //生产线
                string czy = strSb[6].Split('=')[1];  //专业
               

                string dycsj = "";
                if (ex.isDate((strSb[7].Split('=')[1])) == true)
                {
                    dycsj = strSb[7].Split('=')[1].Replace("+", " ");       //异常时间
                }


                string djssj = "";        //结束时间
                if (ex.isDate((strSb[8].Split('=')[1])) == true)
                {
                    djssj = strSb[8].Split('=')[1].Replace("+", " ");
                }

                string czyr = strSb[9].Split('=')[1];    //作业人
                if (string.IsNullOrEmpty(czyr) || czyr.Trim() == "")
                {
                    HttpContext.Current.Response.Write("9");
                    return;
                }

                string cfzry = strSb[10].Split('=')[1];   //辅助人员
                string cycms = strSb[11].Split('=')[1];  //异常描述
                string ccljl = strSb[12].Split('=')[1];  //处理记录
                string cbz = strSb[13].Split('=')[1];   //备注

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@gdh",cgdh),
                            new SqlParameter("@sbmc",csbmc),
                            new SqlParameter("@scxid",cscx),
                            new SqlParameter("@zyid",czy),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@ycms",cycms),
                            new SqlParameter("@ycsj",dycsj),
                            new SqlParameter("@cljl",ccljl),
                            new SqlParameter("@jssj",djssj),
                            new SqlParameter("@zyr",czyr),
                            new SqlParameter("@fzry",cfzry),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz)
                                 };

                cmd.CommandText = "insert_ycjl";
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in parms)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();


                //不进履历表,进值班记录(维修记录),根据设备id是否为0区别，为0标示异常记录
                SqlParameter[] parm_wx = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@gdh",cgdh),
                            new SqlParameter("@sbid",""),
                            new SqlParameter("@sbmc",csbmc),
                            new SqlParameter("@scxid",cscx),
                            new SqlParameter("@zyid",czy),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@gzsj",drq),
                            new SqlParameter("@gzlx",""),
                            new SqlParameter("@gzxx",cycms),
                            new SqlParameter("@clfs",ccljl),
                            new SqlParameter("@wxr",czyr),
                            new SqlParameter("@wxnr",ccljl),
                            new SqlParameter("@wxkssj",""),
                            new SqlParameter("@wcsj",""),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz)
                                 };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "insert_zbjl";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm_wx)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //维修明细
                int rCount = int.Parse(str[1].Split('*')[2]);

                if (rCount > 0)
                {
                    string[] strBj = str[1].Split('*')[1].Split('@');
                    string[] value;
                    string bjbh = "";
                    double sl = 0;
                    string strSql = "";

                    //备件资料
                    for (int i = 0; i < rCount; i++)
                    {
                        value = strBj[i].Split('&');
                        bjbh = value[1];

                        if (bjbh.Trim() == "")
                        {
                            con.Close();
                            HttpContext.Current.Response.Write("4&" + (i + 1));
                            return;
                        }

                        ////判断部件编号是否存在
                        //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
                        //int row = SqlHelper.ExecuteSql(strSql);
                        //if (row == 0)
                        //{
                        //    con.Close();

                        //    HttpContext.Current.Response.Write("5&" + (i + 1));
                        //    return;
                        //}

                        if (value[2].Trim() == "")
                        {
                            con.Close();
                            HttpContext.Current.Response.Write("6&" + (i + 1));
                            return;
                        }

                        if (ex.isNumber(value[2]) == true) sl = double.Parse(value[2]);

                        SqlParameter[] parms_mx = {
                            new SqlParameter("@ycid",id),
                            new SqlParameter("@bjbh",bjbh),
                            new SqlParameter("@sl",sl)
                        };

                        SqlCommand cmd_mx = new SqlCommand();
                        cmd_mx.Connection = con;//获取数据连接
                        cmd_mx.Transaction = sqltra;//，在执行SQL时
                        cmd_mx.CommandText = "insert_ycjl_mx";
                        cmd_mx.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter p in parms_mx)
                        {
                            cmd_mx.Parameters.Add(p);
                        }
                        cmd_mx.ExecuteNonQuery();
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


        ////转单保存
        //private void Add2()
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //遍历获取传递过来的字符串
        //        foreach (string s in HttpContext.Current.Request.Form.AllKeys)
        //        {
        //            sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
        //        }
        //        string ss = sb.ToString();
        //        string[] str = ss.Split('~');
        //        string[] strSb = str[0].Split('&');     //设备资料

        //        DataTable dt = new DataTable();

        //        //查找id最大值
        //        Int64 id = 0;
        //        dt = SqlHelper.GetTable("select max(id) as maxid from ycjlb");
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (!string.IsNullOrEmpty(dt.Rows[0]["maxid"].ToString()))
        //            {
        //                id = Int64.Parse(dt.Rows[0]["maxid"].ToString()) + 1;
        //            }
        //            else
        //            {
        //                id = 1;
        //            }
        //        }
        //        else
        //        {
        //            id = 1;
        //        }
        //        dt = null;

        //        string cgdh = strSb[1].Split('=')[1];  //工单号

        //        dt = SqlHelper.GetTable("select * from ycjlb where cgdh='" + cgdh + "'");
        //        if (dt.Rows.Count > 0)
        //        {
        //            dt = null;
        //            HttpContext.Current.Response.Write("1");
        //            return;
        //        }
        //        dt = null;

        //        sys ex = new sys();
        //        string drq = "";
        //        if (ex.isDate((strSb[2].Split('=')[1])) == true)
        //        {
        //            drq = strSb[2].Split('=')[1];
        //        }

        //        string csbbh = strSb[3].Split('=')[1];  //设备编号

        //        if (string.IsNullOrEmpty(csbbh))
        //        {
        //            HttpContext.Current.Response.Write("2");
        //            return;
        //        }

        //        string isbid = "";
        //        dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
        //        if (dt.Rows.Count == 0)
        //        {
        //            HttpContext.Current.Response.Write("3");
        //            return;
        //        }
        //        else
        //        {
        //            isbid = dt.Rows[0]["id"].ToString();
        //        }

        //        string cscx = strSb[5].Split('=')[1];  //生产线
        //        string czy = strSb[6].Split('=')[1];  //专业
        //        string cycms = strSb[7].Split('=')[1];  //异常描述

        //        string dycsj = "";
        //        if (ex.isDate((strSb[8].Split('=')[1])) == true)
        //        {
        //            dycsj = strSb[8].Split('=')[1].Replace("+", " ");
        //        }


        //        string ccljl = strSb[9].Split('=')[1];  //处理记录
        //        string czyr = strSb[10].Split('=')[1];    //作业人
        //        if (string.IsNullOrEmpty(czyr) || czyr.Trim() == "")
        //        {
        //            HttpContext.Current.Response.Write("9");
        //            return;
        //        }

        //        string djssj = "";        //结束时间
        //        if (ex.isDate((strSb[11].Split('=')[1])) == true)
        //        {
        //            djssj = strSb[11].Split('=')[1].Replace("+", " ");
        //        }

        //        string cbz = strSb[12].Split('=')[1];   //备注

        //        string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
        //        SqlConnection con = new SqlConnection(strConn);
        //        con.Open();

        //        SqlTransaction sqltra = con.BeginTransaction();//开始事务
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;//获取数据连接
        //        cmd.Transaction = sqltra;//，在执行SQL时

        //        SqlParameter[] parms = {
        //                    new SqlParameter("@id",id),
        //                    new SqlParameter("@gdh",cgdh),
        //                    new SqlParameter("@sbid",isbid),
        //                    new SqlParameter("@rq",drq),
        //                    new SqlParameter("@ycms",cycms),
        //                    new SqlParameter("@ycsj",dycsj),
        //                    new SqlParameter("@cljl",ccljl),
        //                    new SqlParameter("@jssj",djssj),
        //                    new SqlParameter("@zyr",czyr),
        //                    new SqlParameter("@bz",cbz)
        //                         };

        //        cmd.CommandText = "insert_ycjl";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        foreach (SqlParameter p in parms)
        //        {
        //            cmd.Parameters.Add(p);
        //        }
        //        cmd.ExecuteNonQuery();

        //        //履历表
        //        SqlParameter[] parm_llb = {
        //                    new SqlParameter("@xmid",id),
        //                    new SqlParameter("@sbid",isbid),
        //                    new SqlParameter("@zysj",drq),
        //                    new SqlParameter("@zyxm","异常"),
        //                    new SqlParameter("@zyry",czyr),
        //                    new SqlParameter("@zynr",cycms)
        //                         };

        //        SqlCommand com = new SqlCommand();
        //        com.Connection = con;//获取数据连接
        //        com.Transaction = sqltra;//，在执行SQL时
        //        com.CommandText = "insert_llb";
        //        com.CommandType = CommandType.StoredProcedure;
        //        foreach (SqlParameter p in parm_llb)
        //        {
        //            com.Parameters.Add(p);
        //        }
        //        com.ExecuteNonQuery();

        //        //待办事项
        //        SqlParameter[] parms_dbsx = {
        //                    new SqlParameter("@wxid",id),
        //                    new SqlParameter ("@rq",drq),
        //                    new SqlParameter("@sbbh",csbbh),
        //                    new SqlParameter("@sxlx","异常"),
        //                    new SqlParameter("@sxnr",dycsj +"," + cycms ),
        //                    new SqlParameter("@clfs",""),
        //                    new SqlParameter("@dbr",""),
        //                    new SqlParameter("@zt","持续中")
        //                         };
        //        SqlCommand comd = new SqlCommand();
        //        comd.Connection = con;//获取数据连接
        //        comd.Transaction = sqltra;//，在执行SQL时
        //        comd.CommandText = "insert_dbsx";
        //        comd.CommandType = CommandType.StoredProcedure;
        //        foreach (SqlParameter p in parms_dbsx)
        //        {
        //            comd.Parameters.Add(p);
        //        }
        //        comd.ExecuteNonQuery();

        //        //维修明细
        //        int rCount = int.Parse(str[1].Split('*')[2]);

        //        if (rCount > 0)
        //        {
        //            string[] strBj = str[1].Split('*')[1].Split('@');
        //            string[] value;
        //            string bjbh = "";
        //            double sl = 0;
        //            string strSql = "";

        //            //备件资料
        //            for (int i = 0; i < rCount; i++)
        //            {
        //                value = strBj[i].Split('&');
        //                bjbh = value[1];

        //                if (bjbh.Trim() == "")
        //                {
        //                    con.Close();
        //                    HttpContext.Current.Response.Write("4&" + (i + 1));
        //                    return;
        //                }

        //                ////判断部件编号是否存在
        //                //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
        //                //int row = SqlHelper.ExecuteSql(strSql);
        //                //if (row == 0)
        //                //{
        //                //    con.Close();

        //                //    HttpContext.Current.Response.Write("5&" + (i + 1));
        //                //    return;
        //                //}

        //                if (value[2].Trim() == "")
        //                {
        //                    con.Close();
        //                    HttpContext.Current.Response.Write("6&" + (i + 1));
        //                    return;
        //                }

        //                if (ex.isNumber(value[2]) == true) sl = double.Parse(value[2]);

        //                SqlParameter[] parms_mx = {
        //                    new SqlParameter("@ycid",id),
        //                    new SqlParameter("@bjbh",bjbh),
        //                    new SqlParameter("@sl",sl)
        //                };

        //                SqlCommand cmd_mx = new SqlCommand();
        //                cmd_mx.Connection = con;//获取数据连接
        //                cmd_mx.Transaction = sqltra;//，在执行SQL时
        //                cmd_mx.CommandText = "insert_ycjl_mx";
        //                cmd_mx.CommandType = CommandType.StoredProcedure;
        //                foreach (SqlParameter p in parms_mx)
        //                {
        //                    cmd_mx.Parameters.Add(p);
        //                }
        //                cmd_mx.ExecuteNonQuery();
        //            }
        //        }

        //        sqltra.Commit();        //提交事务

        //        if (con.State == ConnectionState.Open) con.Close();
        //        con.Dispose();

        //        HttpContext.Current.Response.Write("添加成功！");
        //    }
        //    catch (Exception ex)
        //    {
        //        sys e = new sys();
        //        e.GetLog(ex);
        //    }
        //}

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
                string[] strSb = str[0].Split('&');     //设备资料
                int id = int.Parse(strSb[0].Split('=')[1]);
                string cgdh = strSb[1].Split('=')[1];  //工单号

                DataTable dt = new DataTable();

                dt = SqlHelper.GetTable("select * from ycjlb where cgdh='" + cgdh + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    dt = null;
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                dt = null;

                sys ex = new sys();
                string drq = "";
                if (ex.isDate((strSb[2].Split('=')[1])) == true)
                {
                    drq = strSb[2].Split('=')[1];
                }

                string cczy = strSb[3].Split('=')[1];  //生产线

                string csbmc = strSb[4].Split('=')[1];  //设备名称

                if (string.IsNullOrEmpty(csbmc))
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cscx = strSb[5].Split('=')[1];  //生产线
                string czy = strSb[6].Split('=')[1];  //专业

                string dycsj = "";
                if (ex.isDate((strSb[7].Split('=')[1])) == true)
                {
                    dycsj = strSb[7].Split('=')[1].Replace("+", " ");
                }

                string djssj = "";        //结束时间
                if (ex.isDate((strSb[8].Split('=')[1])) == true)
                {
                    djssj = strSb[8].Split('=')[1].Replace("+", " ");
                }

                string czyr = strSb[9].Split('=')[1];    //作业人
                if (string.IsNullOrEmpty(czyr) || czyr.Trim() == "")
                {
                    HttpContext.Current.Response.Write("9");
                    return;
                }

                string cfzry = strSb[10].Split('=')[1];    //辅助人员
                string cycms = strSb[11].Split('=')[1];  //异常描述
                string ccljl = strSb[12].Split('=')[1];  //处理记录
                string cbz = strSb[13].Split('=')[1];   //备注

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                SqlParameter[] parms = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@gdh",cgdh),
                            new SqlParameter("@sbmc",csbmc),
                            new SqlParameter("@scxid",cscx),
                            new SqlParameter("@zyid",czy),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@ycms",cycms),
                            new SqlParameter("@ycsj",dycsj),
                            new SqlParameter("@cljl",ccljl),
                            new SqlParameter("@jssj",djssj),
                            new SqlParameter("@zyr",czyr),
                            new SqlParameter("@fzry",cfzry),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz)
                                 };

                cmd.CommandText = "update_ycjl";
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in parms)
                {
                    cmd.Parameters.Add(p);
                }
                cmd.ExecuteNonQuery();

                //不进履历表,进值班记录(维修记录),根据设备id是否为0区别，为0标示异常记录
                SqlParameter[] parm_wx = {
                            new SqlParameter("@id",id),
                            new SqlParameter("@gdh",cgdh),
                            new SqlParameter("@sbid",""),
                            new SqlParameter("@sbmc",csbmc),
                            new SqlParameter("@scxid",cscx),
                            new SqlParameter("@zyid",czy),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@gzsj",drq),
                            new SqlParameter("@gzlx",""),
                            new SqlParameter("@gzxx",cycms),
                            new SqlParameter("@clfs",ccljl),
                            new SqlParameter("@wxr",czyr),
                            new SqlParameter("@wxnr",ccljl),
                            new SqlParameter("@wxkssj",""),
                            new SqlParameter("@wcsj",""),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@bz",cbz)
                                 };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "update_zbjl";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm_wx)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //清除原来的异常描述
                SqlParameter[] parm = {
                            new SqlParameter("@ycid",id)
                        };
                SqlCommand cmds = new SqlCommand();
                cmds.Connection = con;//获取数据连接
                cmds.Transaction = sqltra;//，在执行SQL时
                cmds.CommandText = "update_ycjl_mx";
                cmds.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parm)
                {
                    cmds.Parameters.Add(p);
                }
                cmds.ExecuteNonQuery();

                //插入新的维修明细
                int rCount = int.Parse(str[1].Split('*')[2]);

                if (rCount > 0)
                {
                    string[] strBj = str[1].Split('*')[1].Split('@');
                    string[] value;
                    string bjbh = "";
                    double sl = 0;
                    string strSql = "";

                    //备件资料
                    for (int i = 0; i < rCount; i++)
                    {
                        value = strBj[i].Split('&');
                        bjbh = value[1];

                        if (bjbh.Trim() == "")
                        {
                            con.Close();
                            HttpContext.Current.Response.Write("4&" + (i + 1));
                            return;
                        }

                        ////判断部件编号是否存在
                        //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
                        //int row = SqlHelper.ExecuteSql(strSql);
                        //if (row == 0)
                        //{
                        //    con.Close();

                        //    HttpContext.Current.Response.Write("5&" + (i + 1));
                        //    return;
                        //}

                        if (value[2].Trim() == "")
                        {
                            con.Close();
                            HttpContext.Current.Response.Write("6&" + (i + 1));
                            return;
                        }

                        if (ex.isNumber(value[2]) == true) sl = double.Parse(value[2]);

                        SqlParameter[] parms_mx = {
                            new SqlParameter("@ycid",id),
                            new SqlParameter("@bjbh",bjbh),
                            new SqlParameter("@sl",sl)
                        };

                        SqlCommand cmd_mx = new SqlCommand();
                        cmd_mx.Connection = con;//获取数据连接
                        cmd_mx.Transaction = sqltra;//，在执行SQL时
                        cmd_mx.CommandText = "insert_ycjl_mx";
                        cmd_mx.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter p in parms_mx)
                        {
                            cmd_mx.Parameters.Add(p);
                        }
                        cmd_mx.ExecuteNonQuery();
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

        //private void Edit2()
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //遍历获取传递过来的字符串
        //        foreach (string s in HttpContext.Current.Request.Form.AllKeys)
        //        {
        //            sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
        //        }
        //        string ss = sb.ToString();
        //        string[] str = ss.Split('~');
        //        string[] strSb = str[0].Split('&');     //设备资料
        //        int id = int.Parse(strSb[0].Split('=')[1]);
        //        string cgdh = strSb[1].Split('=')[1];  //工单号

        //        DataTable dt = new DataTable();

        //        dt = SqlHelper.GetTable("select * from ycjlb where cgdh='" + cgdh + "' and id<>" + id);
        //        if (dt.Rows.Count > 0)
        //        {
        //            dt = null;
        //            HttpContext.Current.Response.Write("1");
        //            return;
        //        }
        //        dt = null;

        //        //待办事项中是否已插入
        //        dt = SqlHelper.GetTable("select * from dbsxb where csxlx='异常' and iwxid=" + id);
        //        if (dt.Rows.Count > 0)
        //        {
        //            dt = null;
        //            HttpContext.Current.Response.Write("7");
        //            return;
        //        }
        //        dt = null;

        //        sys ex = new sys();
        //        string drq = "";
        //        if (ex.isDate((strSb[2].Split('=')[1])) == true)
        //        {
        //            drq = strSb[2].Split('=')[1];
        //        }

        //        string csbbh = strSb[3].Split('=')[1];  //设备编号

        //        if (string.IsNullOrEmpty(csbbh))
        //        {
        //            HttpContext.Current.Response.Write("2");
        //            return;
        //        }

        //        string isbid = "";
        //        dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
        //        if (dt.Rows.Count == 0)
        //        {
        //            HttpContext.Current.Response.Write("3");
        //            return;
        //        }
        //        else
        //        {
        //            isbid = dt.Rows[0]["id"].ToString();
        //        }

        //        string cscx = strSb[5].Split('=')[1];  //生产线
        //        string czy = strSb[6].Split('=')[1];  //专业
        //        string cycms = strSb[7].Split('=')[1];  //异常描述

        //        string dycsj = "";
        //        if (ex.isDate((strSb[8].Split('=')[1])) == true)
        //        {
        //            dycsj = strSb[8].Split('=')[1].Replace("+", " ");
        //        }

        //        string ccljl = strSb[9].Split('=')[1];  //处理记录
        //        string czyr = strSb[10].Split('=')[1];    //作业人
        //        if (string.IsNullOrEmpty(czyr) || czyr.Trim() == "")
        //        {
        //            HttpContext.Current.Response.Write("9");
        //            return;
        //        }

        //        string djssj = "";        //结束时间
        //        if (ex.isDate((strSb[11].Split('=')[1])) == true)
        //        {
        //            djssj = strSb[11].Split('=')[1].Replace("+", " ");
        //        }

        //        string cbz = strSb[12].Split('=')[1];   //备注

        //        string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
        //        SqlConnection con = new SqlConnection(strConn);
        //        con.Open();

        //        SqlTransaction sqltra = con.BeginTransaction();//开始事务
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = con;//获取数据连接
        //        cmd.Transaction = sqltra;//，在执行SQL时

        //        SqlParameter[] parms = {
        //                    new SqlParameter("@id",id),
        //                    new SqlParameter("@gdh",cgdh),
        //                    new SqlParameter("@sbid",isbid),
        //                    new SqlParameter("@rq",drq),
        //                    new SqlParameter("@ycms",cycms),
        //                    new SqlParameter("@ycsj",dycsj),
        //                    new SqlParameter("@cljl",ccljl),
        //                    new SqlParameter("@jssj",djssj),
        //                    new SqlParameter("@zyr",czyr),
        //                    new SqlParameter("@bz",cbz)
        //                         };

        //        cmd.CommandText = "update_ycjl";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        foreach (SqlParameter p in parms)
        //        {
        //            cmd.Parameters.Add(p);
        //        }
        //        cmd.ExecuteNonQuery();

        //        //履历表
        //        SqlParameter[] parm_llb = {
        //                    new SqlParameter("@xmid",id),
        //                    new SqlParameter("@sbid",isbid),
        //                    new SqlParameter("@zysj",drq),
        //                    new SqlParameter("@zyxm","异常"),
        //                    new SqlParameter("@zyry",czyr),
        //                    new SqlParameter("@zynr",cycms)
        //                         };

        //        SqlCommand com = new SqlCommand();
        //        com.Connection = con;//获取数据连接
        //        com.Transaction = sqltra;//，在执行SQL时
        //        com.CommandText = "insert_llb";
        //        com.CommandType = CommandType.StoredProcedure;
        //        foreach (SqlParameter p in parm_llb)
        //        {
        //            com.Parameters.Add(p);
        //        }
        //        com.ExecuteNonQuery();

        //        //清除原来的异常描述
        //        SqlParameter[] parm = {
        //                    new SqlParameter("@ycid",id)
        //                };
        //        SqlCommand cmds = new SqlCommand();
        //        cmds.Connection = con;//获取数据连接
        //        cmds.Transaction = sqltra;//，在执行SQL时
        //        cmds.CommandText = "update_ycjl_mx";
        //        cmds.CommandType = CommandType.StoredProcedure;
        //        foreach (SqlParameter p in parm)
        //        {
        //            cmds.Parameters.Add(p);
        //        }
        //        cmds.ExecuteNonQuery();


        //        //插入新的维修明细
        //        int rCount = int.Parse(str[1].Split('*')[2]);

        //        if (rCount > 0)
        //        {
        //            string[] strBj = str[1].Split('*')[1].Split('@');
        //            string[] value;
        //            string bjbh = "";
        //            double sl = 0;
        //            string strSql = "";

        //            //备件资料
        //            for (int i = 0; i < rCount; i++)
        //            {
        //                value = strBj[i].Split('&');
        //                bjbh = value[1];

        //                if (bjbh.Trim() == "")
        //                {
        //                    con.Close();
        //                    HttpContext.Current.Response.Write("4&" + (i + 1));
        //                    return;
        //                }

        //                ////判断部件编号是否存在
        //                //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
        //                //int row = SqlHelper.ExecuteSql(strSql);
        //                //if (row == 0)
        //                //{
        //                //    con.Close();

        //                //    HttpContext.Current.Response.Write("5&" + (i + 1));
        //                //    return;
        //                //}

        //                if (value[2].Trim() == "")
        //                {
        //                    con.Close();
        //                    HttpContext.Current.Response.Write("6&" + (i + 1));
        //                    return;
        //                }

        //                if (ex.isNumber(value[2]) == true) sl = double.Parse(value[2]);

        //                SqlParameter[] parms_mx = {
        //                    new SqlParameter("@ycid",id),
        //                    new SqlParameter("@bjbh",bjbh),
        //                    new SqlParameter("@sl",sl)
        //                };

        //                SqlCommand cmd_mx = new SqlCommand();
        //                cmd_mx.Connection = con;//获取数据连接
        //                cmd_mx.Transaction = sqltra;//，在执行SQL时
        //                cmd_mx.CommandText = "insert_ycjl_mx";
        //                cmd_mx.CommandType = CommandType.StoredProcedure;
        //                foreach (SqlParameter p in parms_mx)
        //                {
        //                    cmd_mx.Parameters.Add(p);
        //                }
        //                cmd_mx.ExecuteNonQuery();
        //            }
        //        }

        //        sqltra.Commit();        //提交事务

        //        if (con.State == ConnectionState.Open) con.Close();
        //        con.Dispose();

        //        HttpContext.Current.Response.Write("修改成功！");
        //    }
        //    catch (Exception ex)
        //    {
        //        sys e = new sys();
        //        e.GetLog(ex);
        //    }
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}