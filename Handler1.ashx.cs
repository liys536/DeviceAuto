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
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {



        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            // 下面这句是最重要的，取得HttpPostedFile对象后就可以调用他的SaveAs方法了  
            HttpPostedFile imgFile = HttpContext.Current.Request.Files["imgFile"];
            string savePath = context.Server.MapPath("~/UpLoad/" + imgFile.FileName);
            imgFile.SaveAs(savePath);

            //context.Response.Write("文件上传成功！文件名：" + imgFile.FileName);


            string filename = imgFile.FileName;

            string id=context.Request["uid"];


            SqlParameter[] parms = {
                            new SqlParameter("@lj",filename),
                            new SqlParameter("@id",id)
                                 };
            bool flag = SqlHelper.ExeNonQuery("update_wdgl_lj", CommandType.StoredProcedure, parms);
            if (flag)
            {
                HttpContext.Current.Response.Redirect("wdgl.html");
            }
            else
            {
                HttpContext.Current.Response.Write("上传失败！");
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