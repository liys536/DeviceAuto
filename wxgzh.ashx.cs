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
    /// wxgzh 的摘要说明
    /// </summary>
    public class wxgzh : IHttpHandler
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
                string appid = str[0].Split('=')[1];
                string appsecret = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(appid))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                if (string.IsNullOrEmpty(appsecret))
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@appid",appid),
                            new SqlParameter("@appsecret",appsecret)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_wxgzh", CommandType.StoredProcedure, parms);
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
                DataTable dt = SqlHelper.GetTable("select * from wxgzh");
                string appid = "";
                string appsecret = "";

                if (dt.Rows.Count > 0)
                {
                    appid = dt.Rows[0]["appid"].ToString();
                    appsecret = dt.Rows[0]["appsecret"].ToString();
                }
                dt = null;

                HttpContext.Current.Response.Write(appid + "&" + appsecret);
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