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
    /// common 的摘要说明
    /// </summary>
    public class common : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder();
                string action = context.Request["action"];

                DataTable dt = new DataTable();

                if (action == "1")            //设备状态
                {
                    dt = SqlHelper.GetTable("select * from sbztb");

                    if (dt.Rows.Count > 0)
                    {

                        DataRow[] CRow = dt.Select("1=1");

                        if (CRow.Length > 0)
                        {

                            sb.Append("[");

                            for (int i = 0; i < CRow.Length; i++)
                            {

                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbzt"].ToString() + "\"},");
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);

                            sb.Append("]},");

                            sb = sb.Remove(sb.Length - 2, 2);

                        }


                        context.Response.Write(sb.ToString());
                    }
                }
                else if (action == "2")     //设备等级
                {
                    dt = SqlHelper.GetTable("select * from sbdjb");

                    if (dt.Rows.Count > 0)
                    {

                        DataRow[] CRow = dt.Select("1=1");

                        if (CRow.Length > 0)
                        {

                            sb.Append("[");

                            for (int i = 0; i < CRow.Length; i++)
                            {

                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbdj"].ToString() + "\"},");
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);

                            sb.Append("]},");

                            sb = sb.Remove(sb.Length - 2, 2);

                        }


                        context.Response.Write(sb.ToString());
                    }
                }
                else if (action == "3")            //设备状态
                {
                    dt = SqlHelper.GetTable("select * from sxlxb");

                    if (dt.Rows.Count > 0)
                    {

                        DataRow[] CRow = dt.Select("1=1");

                        if (CRow.Length > 0)
                        {

                            sb.Append("[");

                            for (int i = 0; i < CRow.Length; i++)
                            {

                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csxlx"].ToString() + "\"},");
                            }

                            sb.Replace(',', ' ', sb.Length - 1, 1);

                            sb.Append("]},");

                            sb = sb.Remove(sb.Length - 2, 2);

                        }


                        context.Response.Write(sb.ToString());
                    }
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