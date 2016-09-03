using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeviceAuto
{
    public partial class PDFView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fname = Request.Params["clj"];
            fname = fname.Substring(4, fname.Length - 4);
            if (Request.Browser.Browser.ToLower() != "chrome")
            {

                string filePath = Server.MapPath("UpLoad\\" + fname);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    string FilePost = filePath.Substring(filePath.Length - 3).ToLower();
                    switch (FilePost)
                    {
                        case "pdf":
                            Response.ContentType = "application/PDF";
                            break;
                        case "doc":
                            Response.ContentType = "application/msword";
                            break;
                        case "xls":
                            Response.ContentType = "application/vnd.ms-excel";
                            break;
                        default:
                            Session["ErrorInfo"] = "不支持的文件格式:" + FilePost;
                            Response.Redirect("errPage.html");
                            break;
                    }
                    Response.WriteFile(filePath);
                    Response.Flush();
                    Response.Close();
                    Session.Remove("Report");
                
            }
            else if (Request.Browser.Browser.ToLower() == "chrome")
            {

                string filePath = Server.MapPath("UpLoad\\" + fname);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    string FilePost = filePath.Substring(filePath.Length - 3).ToLower();
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;

                    if (Request.Browser.Browser == "Firefox")
                        System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + "1.pdf");
                    else
                        System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("1.pdf", System.Text.Encoding.UTF8));               

                    using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        byte[] by = new byte[fs.Length];
                        fs.Read(by, 0, by.Length);
                        Response.BinaryWrite(by);
                        Response.AddHeader("Accept-Language", "zh-tw");
                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Length ", by.Length.ToString());
                        System.Web.HttpContext.Current.Response.Flush();
                        System.Web.HttpContext.Current.Response.End();
                    }
                }

        }
    }
}