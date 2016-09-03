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
    /// td_load 的摘要说明
    /// </summary>
    public class td_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                StringBuilder sb = new StringBuilder("");

                sb.Append("[");

                for (int i = 1; i <=16; i++)
                {

                    sb.Append("{\"id\":\"" + i.ToString() + "\",\"text\":\"" + i.ToString() + "\"},");
                }

                sb.Replace(',', ' ', sb.Length - 1, 1);

                sb.Append("]},");

                sb = sb.Remove(sb.Length - 2, 2);

                context.Response.Write(sb.ToString());

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