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
    /// sbzl_sbbj2 的摘要说明
    /// </summary>
    public class sbzl_sbbj2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string sbid = context.Request["sbid"];
                if (!string.IsNullOrEmpty(sbid))
                {
                    //一页显示几行数据
                    string rows = HttpContext.Current.Request["rows"];
                    //当前页
                    string page = HttpContext.Current.Request["page"];

                    string strWhere = " isbid=" + sbid;

                    DataSet duser = SqlHelper.GetList("v_sbzl_bjzl2", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                    DataTable dt1 = duser.Tables[0];
                    //获取数据源
                    DataTable dt = SqlHelper.GetTable("select * from v_sbzl_bjzl2 where " + strWhere);
                    string str = string.Empty;
                    //将数据转换成json格式
                    str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                    HttpContext.Current.Response.Write(str);
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