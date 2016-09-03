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
    /// dxmc_load 的摘要说明
    /// </summary>
    public class dxmc_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string dx = context.Request["dx"];
                if (!string.IsNullOrEmpty(dx))
                {
                    StringBuilder sb = new StringBuilder("");
                    DataTable dt = new DataTable();
                    if (dx == "1")
                    {
                        dt = SqlHelper.GetTable("select * from bmzlb");
                    }
                    else
                    {
                        dt = SqlHelper.GetTable("select * from ygzlb");
                    }

                    if (dt.Rows.Count > 0)
                    {

                        sb.Append("[");

                        if (dx == "1")        //班组  
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                sb.Append("{\"id\":\"" + dt.Rows[i]["id"].ToString() + "\",\"text\":\"" + dt.Rows[i]["cbmmc"].ToString() + "\"},");
                            }
                        }
                        else
                        {               //个人
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                sb.Append("{\"id\":\"" + dt.Rows[i]["id"].ToString() + "\",\"text\":\"" + dt.Rows[i]["cygxm"].ToString() + "\"},");
                            }
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                    }


                    context.Response.Write(sb.ToString());
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