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
    /// login 的摘要说明
    /// </summary>
    public class login : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string action = context.Request["action"];
                if (action == "query")
                {
                    string s = context.Request["data"];
                    string username = s.Split('&')[0];
                    string userpwd = s.Split('&')[1];

                    DataTable dt = new DataTable();
                    dt = SqlHelper.GetTable("select * from ygzlb where cygbh='" + username + "' and cdlmm='" + userpwd + "' and cCzyf='是'");
                    if (dt.Rows.Count == 0)
                    {
                        context.Response.Write("1");
                        return;
                    }
                    else
                    {
                        //使用session 1.引用using System.Web.SessionState;
                        //            2.  实现IRequiresSessionState接口
                        //              3.context.Session["XXX"] = 值;
                        DataTable userdt = new DataTable();
                        userdt = SqlHelper.GetTable("select depid from DataAuthority where userid='" + dt.Rows[0]["id"] + "'");
                        string sqlwhere = "";
                        if (userdt.Rows.Count > 0)
                        {
                            sqlwhere = " and iBmid in (" + userdt.Rows[0][0] + ") ";
                        }
                        else
                        {
                            sqlwhere = " and 1=2 ";
                        }

                        Util.SetLoginsession(true, sqlwhere);
                        context.Session["username"] = username;
                        context.Session["userpwd"] = userpwd;
                        context.Response.Write("2");
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