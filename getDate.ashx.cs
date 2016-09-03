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
    /// getDate 的摘要说明
    /// </summary>
    public class getDate : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string strDate = context.Request["data"];
                if (strDate.IndexOf("年") >= 0) strDate = strDate.Replace("年", "-");
                if (strDate.IndexOf("月") >= 0) strDate = strDate.Replace("月", "-");
                if (strDate.IndexOf("日") >= 0) strDate = strDate.Replace("日", "");
                //if (strDate.IndexOf("星期") >= 0) strDate = strDate.Replace("星期", "");
                //if (strDate.IndexOf("周") >= 0) strDate = strDate.Replace("周", "");
                //if (strDate.IndexOf("一") >= 0) strDate = strDate.Replace("一", "");
                //if (strDate.IndexOf("二") >= 0) strDate = strDate.Replace("二", "");
                //if (strDate.IndexOf("三") >= 0) strDate = strDate.Replace("三", "");
                //if (strDate.IndexOf("四") >= 0) strDate = strDate.Replace("四", "");
                //if (strDate.IndexOf("五") >= 0) strDate = strDate.Replace("五", "");
                //if (strDate.IndexOf("六") >= 0) strDate = strDate.Replace("六", "");


                //非ie浏览器出现日期格式2016-01-14
                if (strDate.IndexOf("/") >= 0) strDate = strDate.Replace("/", "-");

                string sYear = "";
                string sMonth = "";
                string sDay = "";

                sYear = strDate.Split('-')[0];
                sMonth = strDate.Split('-')[1];
                if (sMonth.Length == 1)
                {
                    sMonth = "0" + sMonth;
                }
                sDay = strDate.Split('-')[2];
                if (sDay.Length == 1)
                {
                    sDay = "0" + sDay;
                }

                strDate = sYear + "-" + sMonth + "-" + sDay;

                context.Response.Write(strDate);
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