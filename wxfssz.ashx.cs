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
    /// wxfssz 的摘要说明
    /// </summary>
    public class wxfssz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "save":
                    save();
                    break;
                case "query":
                    query();
                    break;
            }
        }

        //保存
        private void save()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //遍历获取传递过来的字符串
                foreach (string s in HttpContext.Current.Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
                }
                string ss = sb.ToString();
                string[] str = ss.Split('&');
                string cfssjjg = str[0].Split('=')[1];
                string cfsqssj = str[1].Split('=')[1];
                string cfsjssj = str[2].Split('=')[1];

                if (string.IsNullOrEmpty(cfssjjg))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                if (string.IsNullOrEmpty(cfsqssj))
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                if (string.IsNullOrEmpty(cfsjssj))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@scsjjg",cfssjjg),
                            new SqlParameter("@scqssj",cfsqssj),
                            new SqlParameter("@scjssj",cfsjssj)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_wxfssz", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("保存成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("保存失败！");
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        //初始化
        private void query()
        {
            try
            {
                DataTable dt = SqlHelper.GetTable("select * from wxfsszb");
                string cfssjjg = "";
                string cfsqssj = "";
                string cfsjssj = "";
                if (dt.Rows.Count > 0)
                {
                    cfssjjg = dt.Rows[0]["cfssjjg"].ToString();
                    cfsqssj = dt.Rows[0]["cfsqssj"].ToString();
                    cfsjssj = dt.Rows[0]["cfsjssj"].ToString();
                }
                dt = null;

                HttpContext.Current.Response.Write(cfssjjg + "&" + cfsqssj + "," + cfsjssj);
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