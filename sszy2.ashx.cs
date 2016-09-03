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
    /// sszy2 的摘要说明
    /// </summary>
    public class sszy2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string lbid=context.Request["lbid"];
            switch (action)
            {
                case "query":
                    Query(lbid);
                    break;
                case "del":
                    Del();
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
            //string searchType = "";
            //string searchValue = "";
            if (searchType != null && searchValue != null)
            {
                //sb.AppendFormat(" and charindex('{0}',{1})>0", searchValue, searchType);
                sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string bmid)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = GetWhere();
                if (bmid == "1" || string.IsNullOrEmpty(bmid))
                {
                    bmid = "1";
                }

                //使用存储过程及临时表实现递归方式列表
                SqlParameter[] parms = {
                            new SqlParameter("@id",bmid)
                                 };
                SqlHelper.ExeNonQuery("get_bmzl_sszy", CommandType.StoredProcedure, parms);

                DataSet duser = SqlHelper.GetList("v_sszy", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select *  from v_sszy where " + strWhere);
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

        //删除的方法
        private void Del()
        {
            try
            {
                //获取到选中行的id
                string id = HttpContext.Current.Request["id"];
                int count = 0;
                count = SqlHelper.DelData("sszyb", id);
                if (count > 0)
                {
                    HttpContext.Current.Response.Write("共删除了" + count + "条数据");
                }
                else
                {
                    HttpContext.Current.Response.Write("error");
                }
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
                string bm = str[1].Split('=')[1];
                string sszy = str[2].Split('=')[1];
                //重要：将数组最后一维后面的'\n'去掉，否则出现tree无法加载的情况
                sszy = sszy.Substring(0, sszy.Length - 1);

                if (bm.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                if (sszy.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                //判断分类名称是否存在
                DataTable dt = SqlHelper.GetTable("select * from sszyb where csszy='" + sszy + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@bm",bm),
                            new SqlParameter("@sszy",sszy)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_sszy", CommandType.StoredProcedure, parms);
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
                int bm = int.Parse(str[1].Split('=')[1]);
                string sszy = str[2].Split('=')[1];
                sszy = sszy.Substring(0, sszy.Length - 1);

                //上级分类不允许为空
                if (bm.ToString().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                //分类名称不允许为空
                if (sszy.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                //判断分类名称是否存在
                DataTable dt = SqlHelper.GetTable("select * from sszyb where csszy='" + sszy + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@bm",bm),
                            new SqlParameter("@sszy",sszy),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_sszy", CommandType.StoredProcedure, parms);
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