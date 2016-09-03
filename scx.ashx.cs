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
    /// scx 的摘要说明
    /// </summary>
    public class scx : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string action = context.Request["action"];
                if (!string.IsNullOrEmpty(action))
                {
                    StringBuilder sb = new StringBuilder("");
                    DataTable dt = new DataTable();
                    dt = SqlHelper.GetTable("select * from scxb");


                    if (dt.Rows.Count > 0)
                    {
                        DataRow[] CRow = null;

                        if (action == "1")
                        {
                            CRow = dt.Select("");           //部门为顶级时，显示为部门资料
                        }
                        else
                        {
                            CRow = dt.Select("ibmid=" + action);    //部门为子类部门
                        }

                        if (CRow.Length > 0)
                        {

                            sb.Append("[");

                            for (int i = 0; i < CRow.Length; i++)
                            {

                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["cscx"].ToString() + "\"},");
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);

                            sb.Append("]},");

                            sb = sb.Remove(sb.Length - 2, 2);

                        }
                        else
                        {
                            sb.Append("[{\"id\":\"\",\"text\":\"\"} ]");        //为空时加载
                        }

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