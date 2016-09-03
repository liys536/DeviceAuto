using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Text;
using System.Web.SessionState;

namespace DeviceAuto
{
    /// <summary>
    /// qx_load 的摘要说明
    /// </summary>
    public class qx_load : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                //判断权限
                DataTable dt = new DataTable();
                string username = context.Session["username"].ToString();
                string cgnbh = context.Request["data"];
                string action = context.Request["action"];
                cgnbh = cgnbh.Replace("#", "");

                dt = SqlHelper.GetTable("select * from v_menu where cygbh='" + username + "' and cgnbh='" + cgnbh + "'");
                if (dt.Rows.Count == 0)
                {
                    context.Response.Write("err");
                    return;
                }
                if (action == "del")
                {
                    if (dt.Rows[0]["cdel"].ToString() == "0")
                    {
                        context.Response.Write("del");
                        return;
                    }
                }
                if (action == "add")
                {
                    if (dt.Rows[0]["cadd"].ToString() == "0")
                    {
                        context.Response.Write("add");
                        return;
                    }
                }
                if (action == "edit")
                {
                    if (dt.Rows[0]["cedit"].ToString() == "0")
                    {
                        context.Response.Write("edit");
                        return;
                    }
                }
                if (action == "query")
                {
                    if (dt.Rows[0]["cquery"].ToString() == "0")
                    {
                        context.Response.Write("query");
                        return;
                    }
                }
                dt = null;  //释放数据表
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