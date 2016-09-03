using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// dbsx 的摘要说明
    /// </summary>
    public class dbsx : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string sbid = context.Request["sb"];
            string state = context.Request["state"];
            switch (action)
            {
                case "query":
                    Query(sbid,state);
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
                    Search();
                    break;
            }
        }

        //生成检修计划
        private void Search()
        {
            //一页显示几行数据
            string rows = HttpContext.Current.Request["rows"];
            //当前页
            string page = HttpContext.Current.Request["page"];

            if (HttpContext.Current.Request["data"] == "no")
            {
                HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                return;
            }

            string value = HttpContext.Current.Request["data"];
            string[] strr = value.Split('&');

            string s_where = "czt='持续中'";
            string s_csbbh = strr[0].Split('=')[1];
            if (s_csbbh.Trim().Length > 0)
            {
                s_where = s_where + " and csbbh  like '%" + s_csbbh + "%'";
            }

            string s_csbmc = strr[1].Split('=')[1];
            if (s_csbmc.Trim().Length > 0)
            {
                s_where = s_where + " and csbmc like '%" + s_csbmc + "%'";
            }

            string s_cscx = strr[2].Split('=')[1];
            if (s_cscx.Trim().Length > 0)
            {
                s_where = s_where + " and iscxid = '" + s_cscx + "'";
            }

            string s_csszy = strr[3].Split('=')[1];
            if (s_csszy.Trim().Length > 0)
            {
                s_where = s_where + " and izyid='" + s_csszy + "'";
            }

            string s_csxlx = strr[4].Split('=')[1];
            if (s_csxlx.Trim().Length > 0)
            {
                s_where = s_where + " and csxlx like '%" + s_csxlx + "%'";
            }

            string s_cdbr = strr[5].Split('=')[1];
            if (s_cdbr.Trim().Length > 0)
            {
                s_where = s_where + " and cdbr like '%" + s_cdbr + "%'";
            }

            string s_cnr = strr[6].Split('=')[1];
            if (s_cnr.Trim().Length > 0)
            {
                s_where = s_where + " and csxnr like '%" + s_cnr + "%'";
            }

            string s_cclfs = strr[7].Split('=')[1];
            if (s_cclfs.Trim().Length > 0)
            {
                s_where = s_where + " and cclfs like '%" + s_cclfs + "%'";
            }

            DataSet duser = SqlHelper.GetList("v_dbsx", "*", "czt", int.Parse(rows), int.Parse(page), false, false, s_where);
            DataTable dt1 = duser.Tables[0];
            //获取数据源
            DataTable dt = SqlHelper.GetTable("select * from v_dbsx where " + s_where + " order by drq desc");
            string str = string.Empty;
            //将数据转换成json格式
            str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
            HttpContext.Current.Response.Write(str);
        }

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "待办事项" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
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

                string strWhere = "1=1";

                if (!string.IsNullOrEmpty(sState) && sState == "s")
                {
                    if (!string.IsNullOrEmpty(csbid))
                    {
                        strWhere = strWhere + " and isbid=" + csbid;
                    }
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
                        case "设备编码":
                            strWhere = strWhere + " and csbbh like '%" + ctext + "%'";
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
                        case "事项类型":
                            strWhere = strWhere + " and csxlx like '%" + ctext + "%'";
                            break;
                        case "事项内容":
                            strWhere = strWhere + " and csxnr like '%" + ctext + "%'";
                            break;
                        case "处理方式":
                            strWhere = strWhere + " and cclfs like '%" + ctext + "%'";
                            break;
                        case "待办人":
                            strWhere = strWhere + " and cdbr like '%" + ctext + "%'";
                            break;
                        case "制单人":
                            strWhere = strWhere + " and cczy like '%" + ctext + "%'";
                            break;
                        case "状态":
                            strWhere = strWhere + " and czt like '%" + ctext + "%'";
                            break;
                    }

                }

                DataSet duser = SqlHelper.GetList("v_dbsx", "*", "czt", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_dbsx where " + strWhere + " order by drq desc");
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
                count = SqlHelper.DelData("dbsxb", id);
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

                string csbbh = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(csbbh) || csbbh == "")
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                DataTable dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string rq = "";
                sys ex = new sys();
                if (ex.isDate(str[5].Split('=')[1]) == true)
                {
                    rq = str[5].Split('=')[1];
                }

                string csxlx = str[6].Split('=')[1];
                string cczy = str[9].Split('=')[1];
                string csxnr = str[10].Split('=')[1];
                string cclfs = str[11].Split('=')[1];
                string cdbr = str[7].Split('=')[1];
                string czt = str[8].Split('=')[1];

                SqlParameter[] parms = {
                            new SqlParameter("@wxid","0"),
                            new SqlParameter ("@rq",rq),
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@sxlx",csxlx),
                            new SqlParameter("@sxnr",csxnr),
                            new SqlParameter("@clfs",cclfs),
                            new SqlParameter("@dbr",cdbr),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@zt",czt)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_dbsx", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("添加成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("添加失败！");
                }
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

                string csbbh = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(csbbh) || csbbh == "")
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                DataTable dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "' and id<>" + id);
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string rq = "";
                sys ex = new sys();
                if (ex.isDate(str[5].Split('=')[1]) == true)
                {
                    rq = str[5].Split('=')[1];
                }

                string csxlx = str[6].Split('=')[1];
                string cczy = str[9].Split('=')[1];
                string csxnr = str[10].Split('=')[1];
                string cclfs = str[11].Split('=')[1];
                string cdbr = str[7].Split('=')[1];
                string czt = str[8].Split('=')[1];

                SqlParameter[] parms = {
                            new SqlParameter ("@rq",rq),
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@sxlx",csxlx),
                            new SqlParameter("@sxnr",csxnr),
                            new SqlParameter("@clfs",cclfs),
                            new SqlParameter("@dbr",cdbr),
                            new SqlParameter("@czy",cczy),
                            new SqlParameter("@zt",czt),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_dbsx", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("修改成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("修改失败！");
                }
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