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
    /// sb2_load 的摘要说明
    /// </summary>
    public class sb2_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string sbbh = context.Request["data"];
                string value = "";
                if (!string.IsNullOrEmpty(sbbh))
                {
                    DataTable dt = SqlHelper.GetTable("select * from v_sbzl where csbbh='" + sbbh + "'");
                    if (dt.Rows.Count > 0)
                    {
                        value = dt.Rows[0]["csbbh"].ToString() + "&" + dt.Rows[0]["id"].ToString();
                    }
                    else
                    {
                        value = "err";
                    }
                }
                context.Response.Write(value);
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