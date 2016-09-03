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
    /// t_bsz 的摘要说明
    /// </summary>
    public class t_bsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                string zid=context.Request ["zid"];
                dt = SqlHelper.GetTable("select * from bszb where izid=" + zid + " order by cbdz");

                if (dt.Rows.Count > 0)
                {

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {
                        sb.Append("[");

                        for (int j = 0; j < CRow.Length; j++)
                        {
                            sb.Append("{\"id\":\"" + CRow[j]["id"].ToString() + "\",\"text\":\"" + CRow[j]["cbdz"].ToString() + "~~" + CRow[j]["cbmc"].ToString() + "\",\"state\":\"open\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]");

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