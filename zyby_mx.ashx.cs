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
    /// zyby_mx 的摘要说明
    /// </summary>
    public class zyby_mx : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string byid = context.Request["byid"];
            switch (action)
            {
                case "query":
                    Query(byid);
                    break;
            }
        }

        private void Query(string id)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "";
                if (!string.IsNullOrEmpty(id))
                {
                    strWhere = " ibyid=" + id;
                }
                else
                {
                    strWhere = " 1=1";
                }

                DataSet duser = SqlHelper.GetList("v_zyby_mx", "*", "ibyid", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_zyby_mx where " + strWhere);
                string str = string.Empty;
                //将数据转换成json格式
                str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                HttpContext.Current.Response.Write(str);
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