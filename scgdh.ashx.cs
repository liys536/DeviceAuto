using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DeviceAuto
{
    /// <summary>
    /// scgdh 的摘要说明
    /// </summary>
    public class scgdh : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string data = context.Request["data"];
                string[] str = data.Split('&');
                string dqrq = str[0];
                dqrq = dqrq.Replace("年", "-");
                dqrq = dqrq.Replace("月", "-");
                dqrq = dqrq.Replace("日", "");
                string flag = str[1];
                SqlParameter[] parms = {
                            new SqlParameter("@dqrq",dqrq),
                            new SqlParameter("@flag",flag)
                                 };
                DataSet ds = SqlHelper.ExecuteQuery("getGdh", parms);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    context.Response.Write(dt.Rows[0]["gdh"].ToString());
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