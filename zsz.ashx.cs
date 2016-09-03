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
    /// zsz 的摘要说明
    /// </summary>
    public class zsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from zszb order by czdz");

                if (dt.Rows.Count > 0)
                {

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {

                        sb.Append("[{\"id\":\"\",\"text\":\"站信息\",\"children\":[");

                        for (int i = 0; i < CRow.Length; i++)
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["czdz"].ToString() + "~~" + CRow[i]["czmc"].ToString() + "\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]}]");

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