using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.SessionState;

namespace DeviceAuto
{
    /// <summary>
    /// Main 的摘要说明
    /// </summary>
    public class Main : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                DataTable dt = new DataTable();
                string username = context.Session["username"].ToString();
                dt = SqlHelper.GetTable("select * from v_menu where bXsf=1 order by id");
                if (dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    DataRow[] cRow = null;

                    sb.Append("{\"menus\":[");

                    //基础资料
                    cRow = dt.Select("cgnbh like '%B%' and cygbh='" + username + "'");
                    if (cRow.Length > 0)
                    {
                        sb.Append("{\"menuid\":\"B01\",\"menuname\":\"基础资料\",\"menus\":[");

                        for (int i = 0; i < cRow.Length; i++)
                        {
                            if (cRow[i]["cquery"].ToString() == "1")
                            {
                                sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                            }
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);
                        sb.Append("]},");
                    }

                    //记录管理
                    cRow = dt.Select("cgnbh like '%R%' and cygbh='" + username + "'");
                    if (cRow.Length > 0)
                    {
                        sb.Append("{\"menuid\":\"R01\",\"menuname\":\"记录管理\",\"menus\":[");

                        for (int i = 0; i < cRow.Length; i++)
                        {
                            if (cRow[i]["cquery"].ToString() == "1")
                            {
                                sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                            }
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);
                        sb.Append("]},");
                    }



                    if (username == "0000" )
                    {
                        //组织设置
                        cRow = dt.Select("cgnbh like '%Z%'");
                        if (cRow.Length > 0)
                        {
                            for (int i = 0; i < cRow.Length; i++)
                            {
                                if (cRow[i]["cGnbh"].ToString().Length == 3)
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"menus\":[");
                                }
                                else
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                                }
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);
                            sb.Append("]},");
                        }

                        //维护设置
                        cRow = dt.Select("cgnbh like '%W%'");
                        if (cRow.Length > 0)
                        {
                            for (int i = 0; i < cRow.Length; i++)
                            {
                                if (cRow[i]["cGnbh"].ToString().Length == 3)
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"menus\":[");
                                }
                                else
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                                }
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);
                            sb.Append("]},");
                        }

                        //点检设置
                        cRow = dt.Select("cgnbh like '%D%'");
                        if (cRow.Length > 0)
                        {
                            for (int i = 0; i < cRow.Length; i++)
                            {
                                if (cRow[i]["cGnbh"].ToString().Length == 3)
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"menus\":[");
                                }
                                else
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                                }
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);
                            sb.Append("]},");
                        }

                        //系统管理
                        cRow = dt.Select("cgnbh like '%S%'");
                        if (cRow.Length > 0)
                        {
                            for (int i = 0; i < cRow.Length; i++)
                            {
                                if (cRow[i]["cGnbh"].ToString().Length == 3)
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"menus\":[");
                                }
                                else
                                {
                                    sb.Append("{\"menuid\":\"" + cRow[i]["cgnbh"].ToString() + "\",\"menuname\":\"" + cRow[i]["cgnmc"].ToString() + "\",\"url\":\"" + cRow[i]["clj"].ToString() + "\"},");
                                }
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);
                            sb.Append("]},");
                        }
                    }

                    sb.Replace(',', ' ', sb.Length - 1, 1);

                    sb.Append("]}");

                    context.Response.Write(sb.ToString());
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