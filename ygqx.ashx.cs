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
    /// ygqx 的摘要说明
    /// </summary>
    public class ygqx : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string s = context.Request["data"];
            switch (action)
            {
                case "query":
                    Query(s);
                    break;
                case "edit":
                    Edit();
                    break;
                case "del":
                    Del();
                    break;
            }
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string ygid)
        {
            try
            {
                //一页显示几行数据,使用该语句在firefox下一页显示不全。
                //string rows = HttpContext.Current.Request["rows"];
                string rows = "50";
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = " iygid=" + ygid;

                DataTable dt = new DataTable();

                //获取数据源
                dt = SqlHelper.GetTable("select * from v_ygqx where " + strWhere);

                string str = string.Empty;
                //将数据转换成json格式
                str = JSonHelper.CreateJsonParameters(dt, true, dt.Rows.Count);
                HttpContext.Current.Response.Write(str);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        private void Edit()
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
                string[] str;
                int ygid = 0;
                int jsid = 0;

                str = ss.Split('&');
                ygid = int.Parse(str[0].Split('=')[1]);     //员工id

                if (string.IsNullOrEmpty(str[3].Split('=')[1]))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                jsid = int.Parse(str[3].Split('=')[1]);     //角色id

                DataTable dt = SqlHelper.GetTable("select * from jsglb where id=" + jsid);
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@jsid",jsid),
                            new SqlParameter("@ygid",ygid)
                                 };
                SqlHelper.ExeNonQuery("insert_ygqx", CommandType.StoredProcedure, parms);


                HttpContext.Current.Response.Write("授权成功！");
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }


        private void Del()
        {
            try
            {
                //获取到选中行的id
                string id = HttpContext.Current.Request["id"];

                string sql = "delete from ygjsb where iygid=" + id;
                SqlHelper.TransactSql(sql);

                HttpContext.Current.Response.Write("取消授权成功！");
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