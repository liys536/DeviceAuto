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
    /// sbzl_bj 的摘要说明
    /// </summary>
    public class sbzl_bj : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string  action=context.Request["action"];
            switch (action)
            {
                case "query":
                    Query();
                    break;
                case "add":
                    Add();
                    break;
                case "edit":
                    Edit();
                    break;
            }
        }

        /// <summary>
        /// 组合搜索条件
        /// </summary>
        /// <returns></returns>
        private string GetWhere()
        {
            StringBuilder sb = new StringBuilder("1=1");
            string searchType = HttpContext.Current.Request["search_type"] != "" ? HttpContext.Current.Request["search_type"] : string.Empty;
            string searchValue = HttpContext.Current.Request["search_value"] != "" ? HttpContext.Current.Request["search_value"] : string.Empty;
            if (searchType != null && searchValue != null)
            {
                sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            return sb.ToString();
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

                string strWhere = GetWhere();

                DataSet duser = SqlHelper.GetList("v_sbbj", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_sbbj where " + strWhere);
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

        //添加
        private void Add()
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

                string cbjbh = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(cbjbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                DataTable dt = SqlHelper.GetTable("select * from sbbjb where cbjbh='" + cbjbh + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cbjmc = str[2].Split('=')[1];
                if (string.IsNullOrEmpty(cbjmc))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string cggxh = str[3].Split('=')[1];

                string cjscs = str[4].Split('=')[1];
                string csszy = str[5].Split('=')[1];
                string cbjdj = str[6].Split('=')[1];

                string csccj = str[7].Split('=')[1];

                double ibjsl = 0;
                sys ex = new sys();
                if (ex.isNumber(str[8].Split('=')[1]) == true)
                {
                    ibjsl = double.Parse(str[8].Split('=')[1]);
                }
                string csysb = str[9].Split('=')[1];
                string cbz = str[10].Split('=')[1];


                SqlParameter[] parms = {
                            new SqlParameter("@bjbh",cbjbh),
                            new SqlParameter("@bjmc",cbjmc),
                            new SqlParameter("@ggxh",cggxh),
                            new SqlParameter("@sccj",csccj),
                            new SqlParameter("@bjsl",ibjsl),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@jscs",cjscs),
                            new SqlParameter("@sszy",csszy),
                            new SqlParameter("@sysb",csysb),
                            new SqlParameter("@bjdj",cbjdj),
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_sbbj", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("添加成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("添加失败！");
                }
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
                string[] str = ss.Split('&');
                int id = int.Parse(str[0].Split('=')[1]);


                string cbjbh = str[1].Split('=')[1];
                if (string.IsNullOrEmpty(cbjbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                DataTable dt = SqlHelper.GetTable("select * from sbbjb where cbjbh='" + cbjbh + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cbjmc = str[2].Split('=')[1];
                if (string.IsNullOrEmpty(cbjmc))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }
                string cggxh = str[3].Split('=')[1];
                string cjscs = str[4].Split('=')[1];
                string csszy = str[5].Split('=')[1];
                string cbjdj = str[6].Split('=')[1];

                string csccj = str[7].Split('=')[1];

                double ibjsl = 0;
                sys ex = new sys();
                if (ex.isNumber(str[8].Split('=')[1]) == true)
                {
                    ibjsl = double.Parse(str[8].Split('=')[1]);
                }
                string csysb = str[9].Split('=')[1];
                string cbz = str[10].Split('=')[1];


                SqlParameter[] parms = {
                            new SqlParameter("@bjbh",cbjbh),
                            new SqlParameter("@bjmc",cbjmc),
                            new SqlParameter("@ggxh",cggxh),
                            new SqlParameter("@sccj",csccj),
                            new SqlParameter("@bjsl",ibjsl),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@jscs",cjscs),
                            new SqlParameter("@sszy",csszy),
                            new SqlParameter("@bjdj",cbjdj),
                            new SqlParameter("@sysb",csysb),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_sbbj", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("修改成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("修改失败！");
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