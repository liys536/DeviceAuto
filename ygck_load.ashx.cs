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
    /// ygck_load 的摘要说明
    /// </summary>
    public class ygck_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();


                sb.Append("[");

                sb.Append("{\"id\":\"0\",\"text\":\"查看方式\",\"children\":[");

                //
                dt = SqlHelper.GetTable("select * from bmzlb");
                sb.Append("{\"id\":\"0-1\",\"text\":\"按班组\"");
                if (dt.Rows.Count > 0)
                {
                    sb.Append(",\"children\":[");

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {
                        for (int i = 0; i < CRow.Length; i++)
                        {
                            sb.Append("{\"id\":\"0-1-" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cbmmc"].ToString() + "\"},");

                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                    }

                    sb.Append("},");
                }


                dt = SqlHelper.GetTable("select * from zwzlb");
                sb.Append("{\"id\":\"0-2\",\"text\":\"按职位\"");
                if (dt.Rows.Count > 0)
                {
                    sb.Append(",\"children\":[");

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {
                        for (int i = 0; i < CRow.Length; i++)
                        {
                            sb.Append("{\"id\":\"0-2-" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["czw"].ToString() + "\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                    }
                    sb.Append("}");
                }

                sb.Append("]}]");

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