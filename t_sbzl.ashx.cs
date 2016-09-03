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
    /// t_sbzl 的摘要说明
    /// </summary>
    public class t_sbzl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //注释部分为异步加载分类、设备代码
            //*******************************************************************
            //string action=context.Request["action"];
            //StringBuilder sb = new StringBuilder();

            //sb.Append("[");
            //if (string.IsNullOrEmpty(action))
            //{
            //    sb.Append("{\"id\":\"0\",\"text\":\"设备分类\",\"state\":\"closed\"}");
            //}
            //else
            //{
            //    DataTable dt = new DataTable();
            //    dt=SqlHelper.GetTable("select * from sbflb");
            //    DataRow[] CRow = dt.Select("pid=" + action);
            //    if (CRow.Length > 0)
            //    {

            //        for (int i = 0; i < CRow.Length; i++)
            //        {
            //             sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\",\"state\":\"closed\"},");
            //        }

            //        sb.Replace(',', ' ', sb.Length - 1, 1);
            //    }
            //    else
            //    { 
            //        //真正起作用的只有该部分代码，以上部分实验异步加载
            //        //加载设备资料
            //        dt = SqlHelper.GetTable("select * from sbzlb");
            //        DataRow[] dRow = dt.Select("ilbid=" + action);
            //        if (dRow.Length > 0)
            //        {
            //            for (int j = 0; j < dRow.Length; j++)
            //            {
            //                sb.Append("{\"id\":\"" + dRow[j]["id"].ToString() + "\",\"text\":\"" + dRow[j]["DeviceName"].ToString() + "\",\"state\":\"open\"},");
            //            }

            //            sb.Replace(',', ' ', sb.Length - 1, 1);
            //        }
            //    }
            //}
            //sb.Append("]");
            //context.Response.Write(sb.ToString ());
            //*******************************************************************


            try
            {
                StringBuilder sb = new StringBuilder();
                string action = context.Request["action"];

                if (!string.IsNullOrEmpty(action))  //不加该判断语句，容易出现长度不符的错误。
                {

                    DataTable dt = new DataTable();

                    //异步加载设备资料
                    dt = SqlHelper.GetTable("select * from sbzlb");
                    DataRow[] dRow = dt.Select("ilbid=" + action);
                    if (dRow.Length > 0)
                    {
                        sb.Append("[");

                        for (int j = 0; j < dRow.Length; j++)
                        {
                            sb.Append("{\"id\":\"" + dRow[j]["id"].ToString() + "\",\"text\":\"" + dRow[j]["DeviceId"].ToString() + "~~" +  dRow[j]["DeviceName"].ToString() + "\",\"state\":\"open\"},");
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]");

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