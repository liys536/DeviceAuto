using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace DeviceAuto
{
    /// <summary>
    /// jsgl_load 的摘要说明
    /// </summary>
    public class jsgl_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from jsglb");

                if (dt.Rows.Count > 0)
                {
                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {

                        sb.Append("[");

                        sb.Append("{\"id\":\"0\",\"text\":\"角色\"");
                        sb.Append(",\"children\":[");

                        for (int i = 0; i < CRow.Length; i++)
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cjsbm"].ToString() + "-" + CRow[i]["cjsmc"].ToString() + "\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                        sb.Append("}]");

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