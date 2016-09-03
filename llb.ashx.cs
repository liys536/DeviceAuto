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
    /// llb 的摘要说明
    /// </summary>
    public class llb : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string id = context.Request["data"];
            string sbid = context.Request["sb"];
            string state = context.Request["state"];
            switch (action)
            {
                case "query":
                    Query(sbid,state);
                    break;
                case "w":
                    Load_Wx(id);
                    break;
                case "b":
                    Load_By(id);
                    break;
                case "t":
                    Load_Tj(id);
                    break;
                case "x":
                    Load_Xh(id);
                    break;
                case "y":
                    Load_Yc(id);
                    break;
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
            string fn = "履历表" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
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
                else
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                string qsrq = HttpContext.Current.Request["qsrq"];
                if (!string.IsNullOrEmpty(qsrq))
                {
                    strWhere = strWhere + " and dzysj>='" + qsrq + "'";
                }

                string jzrq = HttpContext.Current.Request["jzrq"];
                if (!string.IsNullOrEmpty(jzrq))
                {
                    strWhere = strWhere + " and dzysj<='" + jzrq + "'";
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
                        case "所属专业":
                            strWhere = strWhere + " and csszy like '%" + ctext + "%'";
                            break;
                        case "作业项目":
                            strWhere = strWhere + " and czyxm like '%" + ctext + "%'";
                            break;
                        case "作业人员":
                            strWhere = strWhere + " and czyry like '%" + ctext + "%'";
                            break;
                        case "制单人":
                            strWhere = strWhere + " and cczy like '%" + ctext + "%'";
                            break;
                        case "备注":
                            strWhere = strWhere + " and cbz like '%" + ctext + "%'";
                            break;
                    }

                }

                DataSet duser = SqlHelper.GetList("v_llb", "*", "dzysj", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_llb where " + strWhere );
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
        /// 载入数据
        /// </summary>
        private void Load_Wx(string strId)
        {
            try
            {
                if (string.IsNullOrEmpty(strId))
                {
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from v_wxjl where id=" + strId);
                if (dt.Rows.Count > 0)
                {
                    string str = "";
                    str = dt.Rows[0]["cgdh"].ToString();
                    str = str + "&" + dt.Rows[0]["drq"].ToString();
                    str = str + "&" + dt.Rows[0]["csbbh"].ToString();
                    str = str + "&" + dt.Rows[0]["csbmc"].ToString();
                    str = str + "&" + dt.Rows[0]["cscx"].ToString();
                    str = str + "&" + dt.Rows[0]["czy"].ToString();
                    str = str + "&" + dt.Rows[0]["dgzsj"].ToString();
                    str = str + "&" + dt.Rows[0]["cgzlx"].ToString();
                    str = str + "&" + dt.Rows[0]["cgzxx"].ToString();
                    str = str + "&" + dt.Rows[0]["cclfs"].ToString();
                    str = str + "&" + dt.Rows[0]["cwxr"].ToString();
                    str = str + "&" + dt.Rows[0]["cwxnr"].ToString();
                    str = str + "&" + dt.Rows[0]["dwxkssj"].ToString();
                    str = str + "&" + dt.Rows[0]["dwcsj"].ToString();
                    str = str + "&" + dt.Rows[0]["cbz"].ToString();
                    HttpContext.Current.Response.Write(str);
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        private void Load_By(string strId)
        {
            try
            {
                if (string.IsNullOrEmpty(strId))
                {
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from v_zyby where id=" + strId);
                if (dt.Rows.Count > 0)
                {
                    string str = "";
                    str = dt.Rows[0]["cgdh"].ToString();
                    str = str + "&" + dt.Rows[0]["drq"].ToString();
                    str = str + "&" + dt.Rows[0]["csbbh"].ToString();
                    str = str + "&" + dt.Rows[0]["csbmc"].ToString();
                    str = str + "&" + dt.Rows[0]["cscx"].ToString();
                    str = str + "&" + dt.Rows[0]["czy"].ToString();
                    str = str + "&" + dt.Rows[0]["dbysj"].ToString();
                    str = str + "&" + dt.Rows[0]["cbylx"].ToString();
                    str = str + "&" + dt.Rows[0]["cnrjl"].ToString();
                    str = str + "&" + dt.Rows[0]["dxcbysj"].ToString();
                    str = str + "&" + dt.Rows[0]["cbyr"].ToString();
                    str = str + "&" + dt.Rows[0]["cbz"].ToString();
                    HttpContext.Current.Response.Write(str);
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        private void Load_Tj(string strId)
        {
            try
            {
                if (string.IsNullOrEmpty(strId))
                {
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from v_tjjl where id=" + strId);
                if (dt.Rows.Count > 0)
                {
                    string str = "";
                    str = dt.Rows[0]["cdh"].ToString();
                    str = str + "&" + dt.Rows[0]["dtxrq"].ToString();
                    str = str + "&" + dt.Rows[0]["csbbh"].ToString();
                    str = str + "&" + dt.Rows[0]["csbmc"].ToString();
                    str = str + "&" + dt.Rows[0]["cscx"].ToString();
                    str = str + "&" + dt.Rows[0]["csszy"].ToString();
                    str = str + "&" + dt.Rows[0]["dkjrq"].ToString();
                    str = str + "&" + dt.Rows[0]["dtjrq"].ToString();
                    str = str + "&" + dt.Rows[0]["ctjzl"].ToString();
                    str = str + "&" + dt.Rows[0]["cxxms"].ToString();
                    str = str + "&" + dt.Rows[0]["cjjfa"].ToString();
                    str = str + "&" + dt.Rows[0]["cyyfx"].ToString();
                    str = str + "&" + dt.Rows[0]["cgsdc"].ToString();
                    str = str + "&" + dt.Rows[0]["ctjr"].ToString();
                    str = str + "&" + dt.Rows[0]["cbz"].ToString();
                    HttpContext.Current.Response.Write(str);
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }


        /// <summary>
        /// 载入数据
        /// </summary>
        private void Load_Xh(string strId)
        {
            try
            {
                if (string.IsNullOrEmpty(strId))
                {
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from v_xhqz where id=" + strId);
                if (dt.Rows.Count > 0)
                {
                    string str = "";
                    str = dt.Rows[0]["cdh"].ToString();
                    str = str + "&" + dt.Rows[0]["dsj"].ToString();
                    str = str + "&" + dt.Rows[0]["csbbh"].ToString();
                    str = str + "&" + dt.Rows[0]["csbmc"].ToString();
                    str = str + "&" + dt.Rows[0]["cscx"].ToString();
                    str = str + "&" + dt.Rows[0]["iyz"].ToString();
                    str = str + "&" + dt.Rows[0]["iqzz"].ToString();
                    str = str + "&" + dt.Rows[0]["cyy"].ToString();
                    str = str + "&" + dt.Rows[0]["czt"].ToString();
                    str = str + "&" + dt.Rows[0]["czdr"].ToString();
                    str = str + "&" + dt.Rows[0]["cbz"].ToString();
                    HttpContext.Current.Response.Write(str);
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        private void Load_Yc(string strId)
        {
            try
            {
                if (string.IsNullOrEmpty(strId))
                {
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from v_ycjl where id=" + strId);
                if (dt.Rows.Count > 0)
                {
                    string str = "";
                    str = dt.Rows[0]["cgdh"].ToString();
                    str = str + "&" + dt.Rows[0]["drq"].ToString();
                    str = str + "&" + dt.Rows[0]["csbbh"].ToString();
                    str = str + "&" + dt.Rows[0]["csbmc"].ToString();
                    str = str + "&" + dt.Rows[0]["cscx"].ToString();
                    str = str + "&" + dt.Rows[0]["czy"].ToString();
                    str = str + "&" + dt.Rows[0]["cycms"].ToString();
                    str = str + "&" + dt.Rows[0]["dycsj"].ToString();
                    str = str + "&" + dt.Rows[0]["ccljl"].ToString();
                    str = str + "&" + dt.Rows[0]["czyr"].ToString();
                    str = str + "&" + dt.Rows[0]["djssj"].ToString();
                    str = str + "&" + dt.Rows[0]["cbz"].ToString();
                    HttpContext.Current.Response.Write(str);
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