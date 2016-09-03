using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace DeviceAuto
{
    public class sys
    {
        //判断字符串是否为数字
        public bool isNumber(string txtValue)
        {
            double num = 0;
            if (double.TryParse(txtValue, out num))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //判断字符串是否为日期
        public bool isDate(string txtValue)
        {
            DateTime dt;
            txtValue = txtValue.Replace("+", " ");
            if (DateTime.TryParse(txtValue, out dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public  string GetMD5(string myString)
        {
            string strM = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(myString, "MD5");

            return strM;
        }


        public  void GetLog(Exception ex)
        {
            string fileLogPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileLogName = "\\Log\\log.txt";

            //自动创建文件夹
            FileInfo fi = new FileInfo(fileLogPath + fileLogName);
            var di = fi.Directory;
            if (!di.Exists) di.Create();

            FileInfo finfo = new FileInfo(fileLogPath + fileLogName);
            using (FileStream fs = finfo.OpenWrite())
            {
                //根据上面创建的文件流创建写数据流 
                StreamWriter strwriter = new StreamWriter(fs);
                //设置写数据流的起始位置为文件流的末尾 
                strwriter.BaseStream.Seek(0, SeekOrigin.End);
                //写入相关记录信息
                strwriter.WriteLine("-------------------------------------------------------------");
                strwriter.WriteLine("时间：" + DateTime.Now.ToString());
                strwriter.WriteLine("异常信息：" + ex.Message.ToString ());
                strwriter.WriteLine("异常对象：" + ex.Source.ToString ());
                strwriter.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim().ToString ());
                strwriter.WriteLine("触发方法：" + ex.TargetSite.ToString());
                strwriter.WriteLine("-------------------------------------------------------------");
                strwriter.WriteLine();
                //清空缓冲区内容，并把缓冲区内容写入基础流 
                strwriter.Flush();
                strwriter.Close();
                fs.Close();
            }
        }

        
    }
}