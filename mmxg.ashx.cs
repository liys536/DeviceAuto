using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.SessionState;

namespace DeviceAuto
{
    /// <summary>
    /// mmxg 的摘要说明
    /// </summary>
    public class mmxg : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                StringBuilder sb = new StringBuilder();
                //遍历获取传递过来的字符串
                foreach (string s in HttpContext.Current.Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
                }
                string username = context.Session["username"].ToString();
                string ss = sb.ToString();
                string[] str = ss.Split('&');
                string ymm = str[2].Split('=')[1];
                string xmm = str[3].Split('=')[1];

                DataTable dt = SqlHelper.GetTable("select * from ygzlb where cygbh='" + username + "'");
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["cdlmm"].ToString() != ymm)
                    {
                        context.Response.Write("1");
                        return;
                    }

                    SqlParameter[] parms = {
                            new SqlParameter("@dlmm",xmm),
                            new SqlParameter("@ygbh",username)
                                 };
                    bool flag = SqlHelper.ExeNonQuery("update_ygmm", CommandType.StoredProcedure, parms);
                    if (flag)
                    {
                        context.Response.Write("密码修改成功！");
                    }
                    else
                    {
                        context.Response.Write("密码修改失败！");
                    }
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