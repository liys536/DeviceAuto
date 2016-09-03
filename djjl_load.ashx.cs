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
    /// djjl_load 的摘要说明
    /// </summary>
    public class djjl_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string s = context.Request["TypeName"];

                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();

                if (s == "" || s == null)
                {
                    dt = SqlHelper.GetTable("select * from CabinetRecordType");

                    if (dt.Rows.Count > 0)
                    {

                        DataRow[] CRow = dt.Select("1=1");

                        if (CRow.Length > 0)
                        {

                            sb.Append("[");

                            for (int i = 0; i < CRow.Length; i++)
                            {

                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["TypeName"].ToString() + "\"},");
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);

                            sb.Append("]},");

                            sb = sb.Remove(sb.Length - 2, 2);

                        }

                        context.Response.Write(sb.ToString());
                    }
                }
                else
                {
                    //参数不为空，加载列

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