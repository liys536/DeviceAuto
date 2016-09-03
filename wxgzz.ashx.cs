using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;

namespace DeviceAuto
{
    /// <summary>
    /// wxgzz 的摘要说明
    /// </summary>
    public class wxgzz : IHttpHandler
    {

        string accessToken = "";

        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                //case "send":
                //    send();
                //    break;
                case "getMess":
                    getMess();
                    break;
                case "query":
                    Query();
                    break;
            }
        }

        //public void send()
        //{
        //    string UserOID = "oFgkQuBxdhXIfJkYmefmDwK6IJP4";   //李
        //    //string UserOID = "oFgkQuA-8KEEK5axPbr1OATaDjCA";   //娄
        //    //string UserOID = "oFgkQuKlp-gC78B3QKcH0GoYD_as";   //南
        //    string Message = "hello word!!!";

        //    string responeJsonStr = "{";
        //    responeJsonStr += "\"touser\": \"" + UserOID + "\",";
        //    responeJsonStr += "\"msgtype\": \"text\",";
        //    responeJsonStr += "\"agentid\": \"3\",";
        //    responeJsonStr += "\"text\": {";
        //    responeJsonStr += "  \"content\": \"" + Message + "\"";
        //    responeJsonStr += "},";
        //    responeJsonStr += "\"safe\":\"0\"";
        //    responeJsonStr += "}";

        //    string sCorpID = "wx334def40db584f80";
        //    string corpsecret = "21d612e6021a54e4f640b81d640305f0";

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append ( SendQYMessage(sCorpID, corpsecret, responeJsonStr, Encoding.UTF8));
        //    HttpContext.Current.Response.Write(sb.ToString ()); 
        //}

        /// <summary>
        /// 组合搜索条件
        /// </summary>
        /// <returns></returns>
        private string GetWhere()
        {
            StringBuilder sb = new StringBuilder("1=1");
            string searchType = HttpContext.Current.Request["search_type"] != "" ? HttpContext.Current.Request["search_type"] : string.Empty;
            string searchValue = HttpContext.Current.Request["search_value"] != "" ? HttpContext.Current.Request["search_value"] : string.Empty;
            if (searchType != null && searchValue != null)
            {
                sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取关注者信息
        /// </summary>
        private void getMess()
        {
            string sCorpID = "";
            string corpsecret = "";
            //获取appid，appsecret
            string sql = "select * from wxgzh";
            DataTable dt = SqlHelper.GetTable(sql);
            if (dt.Rows.Count > 0)
            {
                sCorpID = dt.Rows[0]["appid"].ToString();
                corpsecret=dt.Rows [0]["appsecret"].ToString ();
                corpsecret = corpsecret.Substring(0, corpsecret.Length - 1);
            }
            dt = null;

            StringBuilder sb = new StringBuilder();
            string mess=GetFocusMessage(sCorpID, corpsecret, "", Encoding.UTF8);

            int beginW = mess.IndexOf("[");
            int endW = mess.IndexOf("]");

            string mes = mess.Substring(beginW + 1, endW - (beginW + 1));

            string nc = "";
            string openid = "";

            string strCon = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
            SqlConnection con = new SqlConnection(strCon);
            con.Open();

            SqlTransaction sqltra = con.BeginTransaction();//开始事务

            //删除原来的数据
            sql = "delete from wxgzz";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;//获取数据连接
            cmd.Transaction = sqltra;//，在执行SQL时
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            for (int i = 0; i < mes.Split (',').Length ; i++)
            {
                openid=mes.Split(',')[i];
                openid = openid.Replace("\"","");       //过滤掉“

                nc = GetFocusDetails(sCorpID, corpsecret, "", Encoding.UTF8, openid);       //昵称,使用时需要解析
                nc = nc.Replace(",\"", "々\"");      //使用特殊字符替换，因为昵称中可能含有常用字符，造成提取不准确
                string strNickName = nc.Split('々')[2];
                string nickname = strNickName.Substring(12, strNickName.Length - 12 - 1);
                nickname = nickname.Replace("\\", "");

                string strSex = nc.Split('々')[3];
                string sex = strSex.Substring(6, strSex.Length - 5 - 1);
                sex = sex.Replace("\\", "");

                string ssex = "";
                if (sex == "1")
                {
                    ssex = "男";
                }
                else if (sex == "2")
                {
                    ssex = "女";
                }

                string strlanguage = nc.Split('々')[4];
                string language = strlanguage.Substring(12, strlanguage.Length - 12 - 1);
                language = language.Replace("\\", "");

                string strcity = nc.Split('々')[5];
                string city = strcity.Substring(8, strcity.Length - 8 - 1);
                city = city.Replace("\\", "");

                string strprovince = nc.Split('々')[6];
                string province = strprovince.Substring(12, strprovince.Length - 12 - 1);
                province = province.Replace("\\", "");

                string strcountry = nc.Split('々')[7];
                string country = strcountry.Substring(11, strcountry.Length - 11 - 1);
                country = country.Replace("\\", "");

                string strsubscribe_time = nc.Split('々')[9];
                string subscribe_time = strsubscribe_time.Substring(18, strsubscribe_time.Length - 18 - 1);
                subscribe_time = subscribe_time.Replace("\\", "");

                string strRemark = nc.Split('々')[10];
                string remark = strRemark.Substring(10, strRemark.Length - 10 - 1);
                remark = remark.Replace("\\", "");


                SqlParameter[] parms = {
                                new SqlParameter("@openid",openid),
                                new SqlParameter("@nickname",nickname),
                                new SqlParameter("@sex",ssex),
                                new SqlParameter("@language",language),
                                new SqlParameter("@city",city),
                                new SqlParameter("@province",province),
                                new SqlParameter("@country",country),
                                new SqlParameter("@subscribe_time",subscribe_time),
                                new SqlParameter("@remark",remark)
                                     };

                SqlCommand comd = new SqlCommand();
                comd.Connection = con;//获取数据连接
                comd.Transaction = sqltra;//，在执行SQL时
                comd.CommandText = "insert_wxgzz";
                comd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in parms)
                {
                    comd.Parameters.Add(p);
                }
                comd.ExecuteNonQuery();
                comd.Dispose();
            }

            sqltra.Commit();

            if (con.State == ConnectionState.Open) con.Close();
            con.Dispose();

            HttpContext.Current.Response.Write("sucess"); 
        
        }


        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query()
        {
            try
            {
                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = GetWhere();

                DataSet duser = SqlHelper.GetList("wxgzz", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from wxgzz where " + strWhere);
                string str = string.Empty;
                //将数据转换成json格式
                str = JSonHelper.CreateJsonParameters(dt1, true, dt.Rows.Count);
                HttpContext.Current.Response.Write(str);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }


//        调用方法
// List<Person> appResult = JSONToObject<List<Person>>(@"[{""id"":0,""email"":""abc0@gmail.com"",""age"":0},{""id"":1,""email"":""abc1@gmail.com"",""age"":2},{""id"":2,""email"":""abc2@gmail.com"",""age"":4},{""id"":3,""email"":""abc3@gmail.com"",""age"":6},{""id"":4,""email"":""abc4@gmail.com"",""age"":8}]");
     
//     for(int i = 0;i<appResult.Count;i++)
//      输出(appResult[i].id);
 
////定义
//   public class Person
//   {
//     public int id { set; get; }
//     public String email { set; get; }
//     public int age { set; get; }
//   }
//   public static T JSONToObject<T>(string jsonText)
//   {
//     System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
//     try
//     {
//       return jss.Deserialize<T>(jsonText);
//     }
//     catch (Exception ex)
//     {
//       throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
//     }
//   } 

 

        /// <summary>
        /// 获取企业号的accessToken
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public string GetQYAccessToken(string corpid, string corpsecret)
        {
            string accessToken = "";

            string getAccessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

            string respText = "";

            //获取josn数据
            string url = string.Format(getAccessTokenUrl, corpid, corpsecret);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream resStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.Default);
                respText = reader.ReadToEnd();
                resStream.Close();
            }

            try
            {
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(respText);

                //通过键access_token获取值
                accessToken = respDic["access_token"].ToString();
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }

            return accessToken;
        }

        /// <summary>
        /// Post数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();

                //该处用Encoding.UTF-8编码，因为微信使用的就是该编码，不能使用Encoding.Default，中文下容易出现乱码
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
            return ret;
        }

        /// <summary>
        /// 推送信息
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <param name="paramData">提交的数据json</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public string SendQYMessage(string corpid, string corpsecret, string paramData, Encoding dataEncode)
        {
            //string accessToken = GetQYAccessToken(corpid, corpsecret);

            string postUrl = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", accessToken);

            return PostWebRequest(postUrl, paramData, dataEncode);
        }

        /// <summary>
        /// 获取关注者列表
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <param name="paramData">提交的数据json</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public string GetFocusMessage(string corpid, string corpsecret, string paramData, Encoding dataEncode)
        {
            accessToken = GetQYAccessToken(corpid, corpsecret);

            string postUrl = string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}", accessToken);

            return PostWebRequest(postUrl, paramData, dataEncode);
        }


        /// <summary>
        /// 获取关注者详细信息
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <param name="paramData">提交的数据json</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public string GetFocusDetails(string corpid, string corpsecret, string paramData, Encoding dataEncode, string OpenID)
        {
            //string accessToken = GetQYAccessToken(corpid, corpsecret);

            string postUrl = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", accessToken, OpenID);

            return PostWebRequest(postUrl, paramData, dataEncode);
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