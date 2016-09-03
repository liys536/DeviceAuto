using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// ygzl 的摘要说明
    /// </summary>
    public class ygzl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action=context.Request["action"];
            string lbid = context.Request["lbid"];
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
                case "export":
                    Export();
                    break;
                case "edit":
                    Edit();
                    break;
            }
        }
        
        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "员工资料" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data =HttpContext.Current.Request["data"];
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
            if (searchType != null && searchValue !=null)
            {
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
                SqlHelper.ExeNonQuery("get_bmzl2", CommandType.StoredProcedure, parms);

                DataSet duser = SqlHelper.GetList("v_ygzl", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_ygzl where " + strWhere + " order by cygbh asc");
                string str = string.Empty;
                //将数据转换成json格式
                str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                //str = JSonHelper.SerializeObject(dt1);
                //str ="{ " + "\"total\":" + dt.Rows.Count + "," + "\"rows\":" + str + "}";
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
                count = SqlHelper.DelData("ygzlb", id);
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
                string cygbh = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(cygbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from ygzlb where cygbh='" + cygbh + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cygxm = str[2].Split('=')[1];

                if (string.IsNullOrEmpty(cygxm))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }
                string cdlmm = str[3].Split('=')[1];
                int ibmid = 0;
                sys ex = new sys();
                if (ex.isNumber(str[4].Split('=')[1]) == true)
                {
                    ibmid = int.Parse(str[4].Split('=')[1]);
                }
                int izwid = 0;
                if (ex.isNumber(str[5].Split('=')[1]) == true)
                {
                    izwid = int.Parse(str[5].Split('=')[1]);
                }
                string cnl = str[6].Split('=')[1];
                string cxb = str[7].Split('=')[1] == "1" ? "男" : "女";

                string drzsj = "";
                if (ex.isDate(str[8].Split('=')[1]) == true)
                {
                    drzsj = str[8].Split('=')[1];
                }
                string cbyyx = str[9].Split('=')[1];
                string czy = str[10].Split('=')[1];
                string csfzh = str[11].Split('=')[1];
                string clxdh = str[12].Split('=')[1];
                string cjjlxr = str[13].Split('=')[1];
                string cjjlxrdh = str[14].Split('=')[1];
                string cczyf = str[15].Split('=')[1] == "1" ? "是" : "否";

                string cwxh = str[16].Split('=')[1];
                if (cwxh.Trim().Length > 0)
                {
                    //微信号不允许重复
                    dt = SqlHelper.GetTable("select * from ygzlb where cwxh='" + cwxh + "'");
                    if (dt.Rows.Count > 0)
                    {
                        HttpContext.Current.Response.Write("4");
                        return;
                    }
                }

                string czt = str[17].Split('=')[1] == "在职" ? "1" : "0";
                string cbz = str[18].Split('=')[1];

                SqlParameter[] parms = {
                            new SqlParameter("@ygbh",cygbh),
                            new SqlParameter("@ygxm",cygxm),
                            new SqlParameter("@dlmm",cdlmm),
                            new SqlParameter("@bmid",ibmid),
                            new SqlParameter("@zwid",izwid),
                            new SqlParameter("@xb",cxb),
                            new SqlParameter("@nl",cnl),
                            new SqlParameter("@rzsj",drzsj),
                            new SqlParameter("@byyx",cbyyx),
                            new SqlParameter("@zy",czy),
                            new SqlParameter("@sfzh",csfzh),
                            new SqlParameter("@lxdh",clxdh),
                            new SqlParameter("@jjlxr",cjjlxr),
                            new SqlParameter("@jjlxrdh",cjjlxrdh),
                            new SqlParameter("@czyf",cczyf),
                            new SqlParameter("@wxh",cwxh),
                            new SqlParameter("@zt",czt),
                            new SqlParameter("@bz",cbz)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_ygzl", CommandType.StoredProcedure, parms);
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
                string cygbh = str[1].Split('=')[1];

                if (string.IsNullOrEmpty(cygbh))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from ygzlb where cygbh='" + cygbh + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string cygxm = str[2].Split('=')[1];

                if (string.IsNullOrEmpty(cygxm))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string cdlmm = str[3].Split('=')[1];
                int ibmid = int.Parse(str[4].Split('=')[1]);
                int izwid = int.Parse(str[5].Split('=')[1]);
                string cnl = str[6].Split('=')[1];
                string cxb = str[7].Split('=')[1] == "1" ? "男" : "女";
                string drzsj = "";
                sys ex = new sys();
                if (ex.isDate(str[8].Split('=')[1]) == true)
                {
                    drzsj = str[8].Split('=')[1];
                }
                string cbyyx = str[9].Split('=')[1];
                string czy = str[10].Split('=')[1];
                string csfzh = str[11].Split('=')[1];
                string clxdh = str[12].Split('=')[1];
                string cjjlxr = str[13].Split('=')[1];
                string cjjlxrdh = str[14].Split('=')[1];
                string cczyf = str[15].Split('=')[1] == "1" ? "是" : "否";

                string cwxh = str[16].Split('=')[1];
                if (cwxh.Trim().Length > 0)
                {
                    //微信号不允许重复
                    dt = SqlHelper.GetTable("select * from ygzlb where cwxh='" + cwxh + "' and id<>" + id);
                    if (dt.Rows.Count > 0)
                    {
                        HttpContext.Current.Response.Write("4");
                        return;
                    }
                }
                string czt = str[17].Split('=')[1] ;
                string cbz = str[18].Split('=')[1];
                SqlParameter[] parms = {
                            new SqlParameter("@ygbh",cygbh),
                            new SqlParameter("@ygxm",cygxm),
                            new SqlParameter("@dlmm",cdlmm),
                            new SqlParameter("@bmid",ibmid),
                            new SqlParameter("@zwid",izwid),
                            new SqlParameter("@xb",cxb),
                            new SqlParameter("@nl",cnl),
                            new SqlParameter("@rzsj",drzsj),
                            new SqlParameter("@byyx",cbyyx),
                            new SqlParameter("@zy",czy),
                            new SqlParameter("@sfzh",csfzh),
                            new SqlParameter("@lxdh",clxdh),
                            new SqlParameter("@jjlxr",cjjlxr),
                            new SqlParameter("@jjlxrdh",cjjlxrdh),
                            new SqlParameter("@czyf",cczyf),
                            new SqlParameter("@wxh",cwxh),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@zt",czt),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_ygzl", CommandType.StoredProcedure, parms);
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