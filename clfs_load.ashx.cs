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
    /// clfs_load 的摘要说明
    /// </summary>
    public class clfs_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string gzid = context.Request["data"];

                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select id,cclfs from clfsb where igzid=" + gzid);

                if (dt.Rows.Count > 0)
                {

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {

                        sb.Append("[");

                        for (int i = 0; i < CRow.Length; i++)
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cclfs"].ToString() + "\"},");
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