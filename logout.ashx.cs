using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.SessionState;

namespace DeviceAuto
{
    /// <summary>
    /// logout 的摘要说明
    /// </summary>
    public class logout : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                context.Session["username"] = "";   //清空用户session
                context.Session["userpwd"] = "";
            }
            catch (Exception ex)
            {
                sys e = new sys();
               
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