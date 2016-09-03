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
    /// qxgl 的摘要说明
    /// </summary>
    public class qxgl : IHttpHandler
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
                case "power":
                    power();
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
        /// 查询的方法
        /// </summary>
        private void Query(string jsid)
        {
            try
            {
                //一页显示几行数据,使用该语句在firefox下一页显示不全。
                //string rows = HttpContext.Current.Request["rows"];
                string rows = "50";
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "";

                if (string.IsNullOrEmpty(jsid))
                {
                    strWhere = " 1=1 ";
                }
                else
                {
                    if (jsid == "0")
                    {
                        strWhere = " 1>2";
                    }
                    else
                    {
                        strWhere = " ijsid=" + jsid;
                    }
                }

                DataSet duser = new DataSet();
                DataTable dt1 = new DataTable();
                DataTable dt = new DataTable();

                //获取数据源
                dt = SqlHelper.GetTable("select * from v_qxgl where " + strWhere);

                if (dt.Rows.Count > 0)  //加载已有权限
                {
                    duser = SqlHelper.GetList("v_qxgl", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                }
                else
                {
                    dt = SqlHelper.GetTable("select * from v_qxgl2 where " + strWhere);
                    //没有该操作员的权限，加载权限为空
                    duser = SqlHelper.GetList("v_qxgl2", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                }

                dt1 = duser.Tables[0];
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

        private void power()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //遍历获取传递过来的字符串
                foreach (string s in HttpContext.Current.Request.Form.AllKeys)
                {
                    sb.AppendFormat("{0}:{1}\n", s, HttpContext.Current.Request.Form[s]);
                }
                string[] str;
                string cadd = "0";
                string cedit = "0";
                string cdel = "0";
                string cquery = "0";
                string val;
                string cgnbh;
                int ijsid;
                string ss = sb.ToString();
                string[] value = ss.Split('*')[1].Split('@');
                int total = int.Parse(ss.Split('*')[2]);
                for (int i = 0; i < total; i++)
                {
                    str = value[i].Split('&');
                    cgnbh = str[1];
                    ijsid = int.Parse(str[0]);
                    if (str[2] == "1")
                    {
                        cadd = str[2];
                    }
                    else
                    {
                        cadd = "0";
                    }
                    if (str[3] == "1")
                    {
                        cdel = str[3];
                    }
                    else
                    {
                        cdel = "0";
                    }
                    if (str[4] == "1")
                    {
                        cedit = str[4];
                    }
                    else
                    {
                        cedit = "0";
                    }
                    if (str[5] == "1")
                    {
                        cquery = str[5];
                    }
                    else
                    {
                        cquery = "0";
                    }
                    val = cadd + cdel + cedit + cquery;        //权限格式：增删改查

                    SqlParameter[] parms = {
                            new SqlParameter("@jsid",ijsid),
                            new SqlParameter("@gnbh",cgnbh),
                            new SqlParameter("@qx",val)
                                 };
                    SqlHelper.ExeNonQuery("insert_qx", CommandType.StoredProcedure, parms);

                }

                HttpContext.Current.Response.Write("授权成功！");
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
                count = SqlHelper.DelData("jsglb", id);
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
                string jsbm = str[1].Split('=')[1];
                string jsmc = str[2].Split('=')[1];
                //重要：将数组最后一维后面的'\n'去掉，否则出现tree无法加载的情况
                jsmc = jsmc.Substring(0, jsmc.Length - 1);

                if (jsbm.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                if (jsmc.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                //判断角色编码是否存在
                DataTable dt = SqlHelper.GetTable("select * from jsglb where cjsbm='" + jsbm + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@jsbm",jsbm),
                            new SqlParameter("@jsmc",jsmc)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_jsgl", CommandType.StoredProcedure, parms);
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
                string jsbm = str[1].Split('=')[1];
                string jsmc = str[2].Split('=')[1];
                jsmc = jsmc.Substring(0, jsmc.Length - 1);

                //角色编码不允许为空
                if (jsbm.ToString().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                //角色名称不允许为空
                if (jsmc.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                //判断分类名称是否存在
                DataTable dt = SqlHelper.GetTable("select * from jsglb where cjsbm='" + jsbm + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@jsbm",jsbm),
                            new SqlParameter("@jsmc",jsmc),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_jsgl", CommandType.StoredProcedure, parms);
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