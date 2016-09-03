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
    /// zzqzjl 的摘要说明
    /// </summary>
    public class zzqzjl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context) 
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "query":
                    Query();
                    break;
                case "search":
                    search();
                    break;
            }
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query()
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "1=1";

                //使用存储过程及临时表实现递归方式列表
                SqlParameter[] parms = {
                            new SqlParameter ("@where",strWhere)
                                 };
                SqlHelper.ExeNonQuery("proc_getDepCode", CommandType.StoredProcedure, parms);

                DataSet duser = SqlHelper.GetList("v_zzqzjl", "*", "dsj", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_zzqzjl where " + strWhere );
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


        private void search()
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "";

                string ksrq = HttpContext.Current.Request["ksrq"];
                string jsrq = HttpContext.Current.Request["jsrq"];
                string line = HttpContext.Current.Request["line"];
                string nr = HttpContext.Current.Request["nr"];
                //string zdr = HttpContext.Current.Request["zdr"];

                strWhere = " dsj>='" + ksrq + "' and dsj<='" + jsrq + "'  and cyy like '%" + nr + "%'";

                if (line != "")
                {
                    strWhere = strWhere + " and cscx='" + line + "'";
                }

                //if (zdr != "")
                //{
                //    strWhere = strWhere + " and czdr='" + zdr + "'";
                //}

                //使用存储过程及临时表实现递归方式列表
                SqlParameter[] parms = {
                            new SqlParameter ("@where",strWhere)
                                 };
                SqlHelper.ExeNonQuery("proc_getDepCode", CommandType.StoredProcedure, parms);

                DataSet duser = SqlHelper.GetList("v_zzqzjl", "*", "dsj", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_zzqzjl where " + strWhere );
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