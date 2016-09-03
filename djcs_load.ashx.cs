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
    /// djcs_load 的摘要说明
    /// </summary>
    public class djcs_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder();
                string action = context.Request["action"];
                string value = context.Request["txt"];
                string state = context.Request["state"];
                string sbid = context.Request["sbid"];

                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];


                if (state == "1")
                {
                    string strWhere = "";

                    if (!string.IsNullOrEmpty(value))
                    {
                        strWhere = " typename='" + value + "'";
                    }
                    else
                    {
                        strWhere = " 1>2";
                    }

                    DataSet duser = SqlHelper.GetList("v_djcs", "*", "iNumber", int.Parse(rows), int.Parse(page), false, false, strWhere);
                    DataTable dt1 = duser.Tables[0];
                    //获取数据源
                    DataTable dt = SqlHelper.GetTable("select * from v_djcs where " + strWhere );
                    string str = string.Empty;
                    //将数据转换成json格式
                    str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                    HttpContext.Current.Response.Write(str);
                }
                else if (state == "2")
                {
                    string strWhere = "";

                    if (string.IsNullOrEmpty(sbid))
                    {
                        strWhere = " 1=1";
                    }
                    else
                    {
                        strWhere = " isbid=" + sbid;
                    }

                    DataSet duser = SqlHelper.GetList("v_sbzl_djcs", "*", "iNumber", int.Parse(rows), int.Parse(page), false, false, strWhere);
                    DataTable dt1 = duser.Tables[0];
                    //获取数据源
                    DataTable dt = SqlHelper.GetTable("select * from v_sbzl_djcs where " + strWhere);
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