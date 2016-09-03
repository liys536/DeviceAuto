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
    /// sbfl 的摘要说明
    /// </summary>
    public class sbfl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                string action = context.Request["action"];
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from sbflb order by pid");

                if (dt.Rows.Count > 0)
                {

                    if (action == "0")
                    {
                        sb.Append(GetDataString2(dt, "0"));
                    }
                    else
                    {
                        sb.Append(GetDataString(dt, "0"));
                    }

                    sb = sb.Remove(sb.Length - 2, 2);

                }

                context.Response.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }

        }

        public string GetDataString(DataTable dt, string id)
        {

            StringBuilder sb = new StringBuilder();

            DataRow[] CRow = dt.Select("pid=" + id);

            if (CRow.Length > 0)
            {

                sb.Append("[");

                for (int i = 0; i < CRow.Length; i++)
                {

                    string chidstring = GetDataString(dt, CRow[i]["id"].ToString());

                    if (!string.IsNullOrEmpty(chidstring))
                    {

                        sb.Append("{ \"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\",\"state\":\"open\",\"children\":");

                        sb.Append(chidstring);

                    }

                    else
                    {

                        if (int.Parse(CRow[i]["id"].ToString()) % 2 == 0)
                        {
                            //state为closed时折叠
                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\"},");

                        }

                        else
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\"},");

                        }

                    }

                }

                sb.Replace(',', ' ', sb.Length - 1, 1);

                sb.Append("]},");

            }

            return sb.ToString();

        }

        public string GetDataString2(DataTable dt, string id)
        {

            StringBuilder sb = new StringBuilder();

            DataRow[] CRow = dt.Select("pid=" + id);

            if (CRow.Length > 0)
            {

                sb.Append("[");

                for (int i = 0; i < CRow.Length; i++)
                {

                    string chidstring = GetDataString2(dt, CRow[i]["id"].ToString());

                    if (!string.IsNullOrEmpty(chidstring))
                    {

                        sb.Append("{ \"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\",\"state\":\"open\",\"children\":");

                        sb.Append(chidstring);

                    }

                    else
                    {

                        if (int.Parse(CRow[i]["id"].ToString()) % 2 == 0)
                        {
                            //state为closed时折叠
                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\",\"state\":\"closed\"},");

                        }

                        else
                        {

                            sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["csbfl"].ToString() + "\",\"state\":\"closed\"},");

                        }

                    }

                }

                sb.Replace(',', ' ', sb.Length - 1, 1);

                sb.Append("]},");

            }

            return sb.ToString();

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