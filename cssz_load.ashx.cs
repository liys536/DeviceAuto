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
    /// cssz_load 的摘要说明
    /// </summary>
    public class cssz_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string sbid = context.Request["sbid"];
                StringBuilder sb = new StringBuilder("");
                if (!string.IsNullOrEmpty(sbid))
                {

                    DataTable dt = new DataTable();
                    dt = SqlHelper.GetTable("select * from sbzlb s inner join sbzlb_djfs sd on s.id=sd.isbid " +
                                    " inner join CabinetRecordType c on sd.idjid=c.id where s.id=" + sbid);


                    if (dt.Rows.Count > 0)
                    {

                        sb.Append("[");

                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber1"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber1\",\"text\":\"" + dt.Rows[0]["iNumber1"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber2"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber2\",\"text\":\"" + dt.Rows[0]["iNumber2"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber3"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber3\",\"text\":\"" + dt.Rows[0]["iNumber3"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber4"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber4\",\"text\":\"" + dt.Rows[0]["iNumber4"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber5"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber5\",\"text\":\"" + dt.Rows[0]["iNumber5"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber6"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber6\",\"text\":\"" + dt.Rows[0]["iNumber6"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber7"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber7\",\"text\":\"" + dt.Rows[0]["iNumber7"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber8"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber8\",\"text\":\"" + dt.Rows[0]["iNumber8"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber9"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber9\",\"text\":\"" + dt.Rows[0]["iNumber9"].ToString() + "\"},");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["iNumber10"].ToString()))
                        {
                            sb.Append("{\"id\":\"iNumber10\",\"text\":\"" + dt.Rows[0]["iNumber10"].ToString() + "\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                    }
                    else
                    {
                        sb.Append("[{\"id\":\"\",\"text\":\"\"} ]");        //为空时加载
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