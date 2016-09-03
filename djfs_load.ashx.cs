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
    /// djfs_load 的摘要说明
    /// </summary>
    public class djfs_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string action = context.Request["action"];
                string id = context.Request["id"];

                if (action == "query")     //增加状态下加载property控件的json
                {
                    StringBuilder sb = new StringBuilder("");

                    if (id == "" || id == null)
                    {
                        sb.Append("[");

                        for (int i = 1; i <= 10; i++)
                        {
                            sb.Append("{\"name\":\"cStr" + i + "\",\"value\":\"\",\"group\":\"string\",\"editor\":\"text\"},");
                            sb.Append("{\"name\":\"bBool" + i + "\",\"value\":\"\",\"group\":\"bool\",\"editor\":\"text\"},");
                            sb.Append("{\"name\":\"iNumber" + i + "\",\"value\":\"\",\"group\":\"number\",\"editor\":\"text\"},");
                        }
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt = SqlHelper.GetTable("select * from CabinetRecordType");
                        DataRow[] CRow = dt.Select("id=" + id);

                        sb.Append("{\"total\":" + dt.Rows.Count);
                        sb.Append(",\"rows\":[");

                        sb.Append("{\"name\":\"cStr1\",\"value\":\"" + CRow[0]["cStr1"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr2\",\"value\":\"" + CRow[0]["cStr2"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr3\",\"value\":\"" + CRow[0]["cStr3"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr4\",\"value\":\"" + CRow[0]["cStr4"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr5\",\"value\":\"" + CRow[0]["cStr5"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr6\",\"value\":\"" + CRow[0]["cStr6"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr7\",\"value\":\"" + CRow[0]["cStr7"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr8\",\"value\":\"" + CRow[0]["cStr8"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr9\",\"value\":\"" + CRow[0]["cStr9"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"cStr10\",\"value\":\"" + CRow[0]["cStr10"].ToString() + "\",\"group\":\"string\",\"editor\":\"text\"},");

                        sb.Append("{\"name\":\"bBool1\",\"value\":\"" + CRow[0]["bBool1"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool2\",\"value\":\"" + CRow[0]["bBool2"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool3\",\"value\":\"" + CRow[0]["bBool3"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool4\",\"value\":\"" + CRow[0]["bBool4"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool5\",\"value\":\"" + CRow[0]["bBool5"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool6\",\"value\":\"" + CRow[0]["bBool6"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool7\",\"value\":\"" + CRow[0]["bBool7"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool8\",\"value\":\"" + CRow[0]["bBool8"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool9\",\"value\":\"" + CRow[0]["bBool9"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"bBool10\",\"value\":\"" + CRow[0]["bBool10"].ToString() + "\",\"group\":\"bool\",\"editor\":\"text\"},");

                        sb.Append("{\"name\":\"iNumber1\",\"value\":\"" + CRow[0]["iNumber1"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber2\",\"value\":\"" + CRow[0]["iNumber2"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber3\",\"value\":\"" + CRow[0]["iNumber3"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber4\",\"value\":\"" + CRow[0]["iNumber4"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber5\",\"value\":\"" + CRow[0]["iNumber5"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber6\",\"value\":\"" + CRow[0]["iNumber6"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber7\",\"value\":\"" + CRow[0]["iNumber7"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber8\",\"value\":\"" + CRow[0]["iNumber8"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber9\",\"value\":\"" + CRow[0]["iNumber9"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");
                        sb.Append("{\"name\":\"iNumber10\",\"value\":\"" + CRow[0]["iNumber10"].ToString() + "\",\"group\":\"number\",\"editor\":\"text\"},");

                    }


                    sb.Replace(',', ' ', sb.Length - 1, 1);

                    sb.Append("]},");

                    sb = sb.Remove(sb.Length - 2, 2);

                    if (id != null)
                    {
                        sb.Append("}");
                    }

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