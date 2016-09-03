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
    /// bj_load 的摘要说明
    /// </summary>
    public class bj_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string bjbh = context.Request["bjbh"];
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from v_sbbj where cbjbh='" + bjbh + "'");

                if (dt.Rows.Count > 0)
                {

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {
                        string id = CRow[0]["id"].ToString();
                        string bjmc = CRow[0]["cbjmc"].ToString();
                        string ggxh = CRow[0]["cggxh"].ToString();
                        string jscs = CRow[0]["cjscs"].ToString();
                        string sszy = CRow[0]["csszy"].ToString();
                        string bjdj = CRow[0]["cbjdj"].ToString();
                        string sccj = CRow[0]["csccj"].ToString();
                        string bz = CRow[0]["cbz"].ToString();

                        sb.Append(id + "&" + bjmc + "&" + ggxh + "&" + jscs + "&" + sszy + "&" + bjdj + "&" + sccj + "&" + bz);
                    }


                    context.Response.Write(sb.ToString());
                }
                else
                {
                    context.Response.Write("err");
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