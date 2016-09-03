using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace DeviceAuto
{
    /// <summary>
    /// sbzl_edit 的摘要说明
    /// </summary>
    public class sbzl_edit : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string sbid=context.Request["data"];

            switch (action)
            {
                case "query":
                    Edit(sbid);
                    break;
            }
        }

        private void Edit(string deviceid)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string value = "";
                string sql = "select * from v_sbzl where csbsbh='" + deviceid  + "'";
                DataTable dt = SqlHelper.GetTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["id"].ToString();
                    string csbfl=dt.Rows[0]["csbfl"].ToString ();
                    string csbbh = dt.Rows[0]["csbbh"].ToString();
                    string csbmc = dt.Rows[0]["csbmc"].ToString();
                    string csbsbh=dt.Rows[0]["csbsbh"].ToString ();
                    string cbmmc=dt.Rows[0]["cbmmc"].ToString ();
                    string csszy=dt.Rows[0]["csszy"].ToString ();
                    string cscx=dt.Rows[0]["cscx"].ToString ();
                    string cggxh = dt.Rows[0]["csbxh"].ToString();
                    string csdwz = dt.Rows[0]["csdwz"].ToString();
                    string cjscs = dt.Rows[0]["cjscs"].ToString();
                    string csbzt = dt.Rows[0]["csbzt"].ToString();
                    string csbdj = dt.Rows[0]["csbdj"].ToString();
                    string csccj = dt.Rows[0]["csccj"].ToString();
                    string izjsl = dt.Rows[0]["izjsl"].ToString();
                    string cazdd = dt.Rows[0]["cazdd"].ToString();
                    string dtyrq = dt.Rows[0]["dtyrq"].ToString();
                    string csjsb = dt.Rows[0]["csjsb"].ToString();
                    string cbz = dt.Rows[0]["cbz"].ToString();

                    value = "?id=" + id + "&csbfl=" + csbfl + "&csbbh=" + csbbh + "&csbmc=" + csbmc + "&csbsbh=" + csbsbh
                           + "&cbmmc=" + cbmmc + "&csszy=" + csszy + "&cscx=" + cscx + "&cggxh=" + cggxh + "&csdwz=" + csdwz
                           + "&cjscs=" + cjscs + "&csbzt=" + csbzt + "&csbdj=" + csbdj + "&csccj=" + csccj + "&izjsl=" + izjsl
                           + "&cazdd=" + cazdd + "&dtyrq=" + dtyrq + "&csjsb=" + csjsb + "&cbz=" + cbz;

                }
                sb.Append(value);

                HttpContext.Current.Response.Write(sb.ToString ());
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("err");
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