using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using System.Diagnostics;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// scsz 的摘要说明
    /// </summary>
    public class scsz : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "save":
                    save();
                    break;
                case "query":
                    query();
                    break;
                case "start":
                    start();
                    break;
                case "stop":
                    stop();
                    break;
            }
        }

        //保存
        private void save()
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
                string cscsjjg = str[0].Split('=')[1];
                string cscqssj = str[1].Split('=')[1];
                string cscjssj = str[2].Split('=')[1];

                if (string.IsNullOrEmpty(cscsjjg))
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                if (string.IsNullOrEmpty(cscqssj))
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                if (string.IsNullOrEmpty(cscjssj))
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                SqlParameter[] parms = {
                            new SqlParameter("@scsjjg",cscsjjg),
                            new SqlParameter("@scqssj",cscqssj),
                            new SqlParameter("@scjssj",cscjssj)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_scsz", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("保存成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("保存失败！");
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        //初始化
        private void query()
        {
            try
            {
                DataTable dt = SqlHelper.GetTable("select * from scszb");
                string cscsjjg = "";
                string cscqssj = "";
                string cscjssj = "";
                if (dt.Rows.Count > 0)
                {
                    cscsjjg = dt.Rows[0]["cscsjjg"].ToString();
                    cscqssj = dt.Rows[0]["cscqssj"].ToString();
                    cscjssj = dt.Rows[0]["cscjssj"].ToString();
                }
                dt = null;

                HttpContext.Current.Response.Write(cscsjjg + "&" + cscqssj + "," + cscjssj);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        private void start()
        {
            try
            {
                DataTable dt = SqlHelper.GetTable("select * from scszb");
                if (dt.Rows.Count > 0)
                {
                    //RunBat(dt.Rows[0]["clj"].ToString() + "\\start.bat");

                    ExecuteRemote("192.168.1.34", "LXH", "admin", "", dt.Rows[0]["clj"].ToString() + "\\start.bat");

                    HttpContext.Current.Response.Write(dt.Rows[0]["clj"].ToString() + "\\start.bat");
                }
                else
                {
                    HttpContext.Current.Response.Write("启动服务失败！");
                }

                dt = null;
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        private void RunBat(string batPath)
        {

            Process pro = new Process();



            FileInfo file = new FileInfo(batPath);

            pro.StartInfo.WorkingDirectory = file.Directory.FullName;

            pro.StartInfo.FileName = batPath;

            pro.StartInfo.CreateNoWindow = false;

            pro.Start();

            pro.WaitForExit();

        }

        private void stop()
        {
            try
            {
                DataTable dt = SqlHelper.GetTable("select * from scszb");
                if (dt.Rows.Count > 0)
                {
                   RunBat(dt.Rows[0]["clj"].ToString() + "\\stop.bat");
                    HttpContext.Current.Response.Write("停止服务成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("停止服务失败！");
                }

                dt = null;
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }


        /// <summary>        
        ///调用远程执行程序命令的方法         
        /// </summary>        
        /// <param name="serverHostName">远程机名或者IP</param>        
        /// <param name="userName">用户名</param>        
        /// <param name="password">帐户密码</param>        
        /// <param name="strCommand">命令</param>        
        /// <param name="path">路径</param>        
        static void ExecuteRemote(string serverHostName, string userName, string password, string strCommand, string path)        
        {            
            //ConnectionOptions指定生成wmi连接所需的设置           
            ConnectionOptions connOption = new ConnectionOptions();            
            connOption.Username = serverHostName + "//" + userName;           
            connOption.Password = password;             
            //ManagementPath 包装了生成和分析wmi对象的路径            
            ManagementPath mngPath = new ManagementPath(@"\\" + serverHostName + @"\root\cimv2:Win32_Process");           
            ManagementScope scope = new ManagementScope(mngPath, connOption);            
            scope.Connect();            
            //ObjectGetOptions 类是指定用于获取管理对象的选项            
            ObjectGetOptions objOption = new ObjectGetOptions();            
            //ManagementClass 是表示公共信息模型 (CIM) 管理类，通过该类的成员，可以使用特定的 WMI 类路径访问 WMI 数据            
            ManagementClass classInstance = new ManagementClass(scope, mngPath, objOption);            
            ManagementBaseObject inParams = classInstance.GetMethodParameters("Create");            
            //设定命令行参数            
            inParams["CommandLine"] = path;            
            ManagementBaseObject outParams = classInstance.InvokeMethod("Create", inParams, null);        
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