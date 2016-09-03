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
    /// sbfl2 的摘要说明
    /// </summary>
    public class sbfl2 : IHttpHandler
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
        private void Query(string flid)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = GetWhere();
                if (flid == "1" || string.IsNullOrEmpty(flid))
                {
                    flid = "1";
                }

                //使用存储过程及临时表实现递归方式列表
                SqlParameter[] parms = {
                            new SqlParameter("@id",flid)
                                 };
                SqlHelper.ExeNonQuery("get_sbfl", CommandType.StoredProcedure, parms);

                DataSet duser = SqlHelper.GetList("v_sbfl", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select *  from v_sbfl where " + strWhere);
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

                string sSql = "";
                DataTable dt = new DataTable();

                //判断设备资料中是否存在
                sSql = "select * from sbzlb where ilbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                //判断该分类是否有下级分类
                sSql = "select * from sbflb where pid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                int count = 0;
                count = SqlHelper.DelData("sbflb", id);
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
                string sjfl = str[1].Split('=')[1];
                string flmc = str[2].Split('=')[1];
                //重要：将数组最后一维后面的'\n'去掉，否则出现tree无法加载的情况
                flmc = flmc.Substring(0, flmc.Length - 1);

                if (sjfl.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                if (flmc.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                //判断分类名称是否存在
                DataTable dt = SqlHelper.GetTable("select * from sbflb where csbfl='" + flmc + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@sjfl",sjfl),
                            new SqlParameter("@flmc",flmc)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_sbfl", CommandType.StoredProcedure, parms);
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
                int sjfl = int.Parse(str[1].Split('=')[1]);
                string flmc = str[2].Split('=')[1];
                flmc = flmc.Substring(0, flmc.Length - 1);

                //上级分类不允许为空
                if (sjfl.ToString().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                //分类名称不允许为空
                if (flmc.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                //判断分类名称是否存在
                DataTable dt = SqlHelper.GetTable("select * from sbflb where csbfl='" + flmc + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }
                //上级分类与分类名称不允许相同
                if (id == sjfl)
                {
                    HttpContext.Current.Response.Write("4");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@sjfl",sjfl),
                            new SqlParameter("@flmc",flmc),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_sbfl", CommandType.StoredProcedure, parms);
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