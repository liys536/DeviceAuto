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
    /// ygzl_load 的摘要说明
    /// </summary>
    public class ygzl_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string action = context.Request["action"];
                string bmid = context.Request["bmid"];
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from ygzlb");

                if (dt.Rows.Count > 0)
                {
                    DataRow[] CRow = dt.Select("1=1");
                    if (action == "1")
                    {
                        if (!string.IsNullOrEmpty(bmid))
                        {
                            CRow = dt.Select("ibmid=" + bmid);
                        }
                        else
                        {
                            CRow = dt.Select("1=1");
                        }

                    }
                    else if (action == "2")
                    {
                        CRow = dt.Select("cczyf='是' and cygbh<>'0000'");        //0000为系统管理员
                    }

                    if (CRow.Length > 0)
                    {

                        sb.Append("[");

                        if (action == "2")
                        {
                            sb.Append("{\"id\":\"0\",\"text\":\"员工资料\"");
                            sb.Append(",\"children\":[");
                        }

                        for (int i = 0; i < CRow.Length; i++)
                        {
                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cygbh"].ToString() + "-" + CRow[i]["cygxm"].ToString() + "\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

                        if (action == "2")
                        {
                            sb.Append("}]");
                        }

                    }
                    else
                    {
                        sb.Append("{\"id\":\"\",\"text\":\"\"}");
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