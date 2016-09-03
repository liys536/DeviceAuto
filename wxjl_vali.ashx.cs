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
    /// wxjl_vali 的摘要说明
    /// </summary>
    public class wxjl_vali : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string data=context.Request["data"];
            if (string.IsNullOrEmpty(data))
            {
                context.Response.Write("1");
                return;
            }

            //判断是否存在该故障的处理方式
            DataTable dt = SqlHelper.GetTable("select * from clfsb where igzid=" + data);
            if (dt.Rows.Count == 0)
            {
                context.Response.Write("2");
                return;
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