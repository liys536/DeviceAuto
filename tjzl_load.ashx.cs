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
    /// tjzl_load 的摘要说明
    /// </summary>
    public class tjzl_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from tjzlb order by pid");

                if (dt.Rows.Count > 0)
                {

                    DataRow[] CRow = dt.Select("1=1");

                    if (CRow.Length > 0)
                    {

                        sb.Append("[");

                        string action = context.Request["action"];

                        for (int i = 0; i < CRow.Length; i++)
                        {

                            if (action == "1")   //显示全部分类
                            {
                                sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["ctjzl"].ToString() + "\"},");
                            }
                            else if (action == "2")      //查找节点是否有子节点，显示最末级分类
                            {
                                DataRow[] dRow = dt.Select("pid=" + CRow[i]["id"]);

                                if (dRow.Length == 0)
                                {
                                    sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"text\":\"" + CRow[i]["ctjzl"].ToString() + "\"},");
                                }
                            }
                        }

                        sb.Replace(',', ' ', sb.Length - 1, 1);

                        sb.Append("]},");

                        sb = sb.Remove(sb.Length - 2, 2);

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