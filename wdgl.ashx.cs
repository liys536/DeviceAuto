using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// wdgl 的摘要说明
    /// </summary>
    public class wdgl : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string action = context.Request["action"];
            string sbid=context.Request["sbid"];
            switch (action)
            {
                case "query":
                    Query(sbid);
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
                case "export":
                    Export();
                    break;
                case "down":
                    Down();
                    break;
            }
        }

        //ie下运行正常，火狐或谷歌不运行
        private void Down()
        {
            string id = HttpContext.Current.Request["data"];

            string fn = "";
            DataTable dt = SqlHelper.GetTable("select * from wdglb where id=" + id);
            if (dt.Rows[0]["clj"].ToString().Length > 0)
            {
                fn = "/UpLoad/" + dt.Rows[0]["clj"].ToString();

                HttpContext.Current.Response.Write(fn);//返回文件名提供下载
            }
            else
            {
                HttpContext.Current.Response.Write("err");
            }

        }

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "文档管理" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data = HttpContext.Current.Request["data"];
            File.WriteAllText(HttpContext.Current.Server.MapPath(fn), data, Encoding.UTF8);//如果是gb2312的xml申明，第三个编码参数修改为Encoding.GetEncoding(936)

            HttpContext.Current.Response.Write(fn);//返回文件名提供下载
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
        private void Query(string csbid)
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = GetWhere();

                if (!string.IsNullOrEmpty(csbid))
                {
                    strWhere = strWhere + " and isbid=" + csbid;
                }

                DataSet duser = SqlHelper.GetList("v_wdgl", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_wdgl where " + strWhere);
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
                count = SqlHelper.DelData("wdglb", id);
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

                DataTable dt = new DataTable();

                string csbbh = str[2].Split('=')[1];  //设备编号

                if (string.IsNullOrEmpty(csbbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                string isbid = "";
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                else
                {
                    isbid = dt.Rows[0]["id"].ToString();
                }

                string cnr = str[1].Split('=')[1];  //内容

                if (cnr == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }


                SqlParameter[] parms = {
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@nr",cnr)
                                 };

                bool flag = SqlHelper.ExeNonQuery("insert_wdgl", CommandType.StoredProcedure, parms);
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
                DataTable dt = new DataTable();

                string csbbh = str[2].Split('=')[1];  //设备编号

                if (string.IsNullOrEmpty(csbbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                string isbid = "";
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csbbh + "' and id<>" + id);
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                else
                {
                    isbid = dt.Rows[0]["id"].ToString();
                }

                string cnr = str[1].Split('=')[1];  //内容

                if (cnr == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@sbid",isbid),
                            new SqlParameter("@nr",cnr),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_wdgl", CommandType.StoredProcedure, parms);
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