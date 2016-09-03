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
    /// dw_load 的摘要说明
    /// </summary>
    public class dw_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from dwb");

                if (dt.Rows.Count > 0)
                {
                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {
                        sb.Append("[");

                        for (int i = 0; i < CRow.Length; i++)
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cdw"].ToString() + "\"},");
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