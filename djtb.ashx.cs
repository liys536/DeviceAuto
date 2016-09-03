using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DeviceAuto
{
    /// <summary>
    /// djtb 的摘要说明
    /// </summary>
    public class djtb : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action=context.Request["action"];
            string cs=context.Request["cs"];
            string qsrq=context.Request["qsrq"];        //开始日期
            string jzrq = context.Request["jzrq"];      //截止日期
            string sbid=context.Request["sbid"];        //设备id
            switch (action)
            {
                case "query":
                    Query(qsrq,jzrq,sbid);
                    break;
                case "query2":
                    Query2(qsrq,jzrq,sbid,cs);
                    break;
            }
        }

        /// <summary>
        /// 查询的方法:获取x轴数据
        /// </summary>
        private void Query(string sks, string sjz,string ssb)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataTable dt = SqlHelper.GetTable("select * from v_djtb where isbid=" + ssb + " and drq>='" + sks + "' and drq<='" + sjz + "' order by drq");
                if (dt.Rows.Count > 0)
                {
                    string dat = "";
                    sb.Append("[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dat = dt.Rows[i]["drq"].ToString();
                        dat = DateTime.Parse(dat).ToString();
                        sb.Append("'" + dat + "',");
                    }
                    sb.Replace(',', ' ', sb.Length - 1, 1);
                    sb.Append("]");
                }
                HttpContext.Current.Response.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        /// <summary>
        /// 查询的方法:获取y轴数据
        /// </summary>
        private void Query2(string sks, string sjz, string ssb, string ccs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataTable dt = SqlHelper.GetTable("select * from v_djtb where isbid=" + ssb + " and drq>='" + sks + "' and drq<='" + sjz + "' order by drq");

                if (dt.Rows.Count > 0)
                {
                    string dat = "0";
                    string sbName = dt.Rows[0]["csbmc"].ToString();
                    
                    sb.Append("[{ name:'" + sbName + "',data:[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dat = dt.Rows[i][ccs].ToString();
                        sb.Append(dat + ",");
                    }

                    sb.Replace(',', ' ', sb.Length - 1, 1);
                    sb.Append("]}]");
                }

                HttpContext.Current.Response.Write(sb.ToString());
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