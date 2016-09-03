using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// HTMLPage1 的摘要说明
    /// </summary>
    public class sbzl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string slbid = context.Request["lb"];
            string action = context.Request["action"];

            switch (action)
            {
                case "query":
                    Query(slbid);
                    break;
                case "del":
                    Del();
                    break;
                case "add":
                    Add();
                    break;
                case "edit":
                    Edit();
                    break;
                case "export":
                    Export();
                    break;
                case "search":
                    Search(slbid);
                    break;
            }
        }

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "设备资料" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data = HttpContext.Current.Request["data"];
            File.WriteAllText(HttpContext.Current.Server.MapPath(fn), data, Encoding.UTF8);//如果是gb2312的xml申明，第三个编码参数修改为Encoding.GetEncoding(936)

            HttpContext.Current.Response.Write(fn);//返回文件名提供下载
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
                        //sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() + "\",\"children\":");

                        sb.Append("{ \"pid\":\"" + CRow[i]["pid"].ToString() + "\",\"csjsb\":\"" + CRow[i]["csjsb"].ToString() +
                            "\",\"id\":\"" + CRow[i]["id"].ToString() + "\",\"ilbid\":\"" + CRow[i]["ilbid"].ToString() +
                            "\",\"iscxid\":\"" + CRow[i]["iscxid"].ToString() + "\",\"izyid\":\"" + CRow[i]["izyid"].ToString() +
                            "\",\"ibmid\":\"" + CRow[i]["ibmid"].ToString() + "\",\"csbfl\":\"" + CRow[i]["csbfl"].ToString() +
                            "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() +
                            "\",\"csbsbh\":\"" + CRow[i]["csbsbh"].ToString() + "\",\"cbmmc\":\"" + CRow[i]["cbmmc"].ToString() +
                            "\",\"cscx\":\"" + CRow[i]["cscx"].ToString() + "\",\"csszy\":\"" + CRow[i]["csszy"].ToString() +
                            "\",\"csbzt\":\"" + CRow[i]["csbzt"].ToString() + "\",\"csbdj\":\"" + CRow[i]["csbdj"].ToString() +
                            "\",\"csbxh\":\"" + CRow[i]["csbxh"].ToString() + "\",\"cjscs\":\"" + CRow[i]["cjscs"].ToString() +
                            "\",\"csccj\":\"" + CRow[i]["csccj"].ToString() + "\",\"izjsl\":\"" + CRow[i]["izjsl"].ToString() +
                            "\",\"cazdd\":\"" + CRow[i]["cazdd"].ToString() + "\",\"dtyrq\":\"" + CRow[i]["dtyrq"].ToString() +
                           "\",\"csdwz\":\"" + CRow[i]["csdwz"].ToString() + "\",\"cbz\":\"" + CRow[i]["cbz"].ToString()  +"\",\"state\":\"open\",\"children\":");

                        sb.Append(chidstring);

                    }

                    else
                    {

                        if (int.Parse(CRow[i]["id"].ToString()) % 2 == 0)
                        {
                            //sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() + "\"},");

                            sb.Append("{ \"pid\":\"" + CRow[i]["pid"].ToString() + "\",\"csjsb\":\"" + CRow[i]["csjsb"].ToString() +
                            "\",\"id\":\"" + CRow[i]["id"].ToString() + "\",\"ilbid\":\"" + CRow[i]["ilbid"].ToString() +
                            "\",\"iscxid\":\"" + CRow[i]["iscxid"].ToString() + "\",\"izyid\":\"" + CRow[i]["izyid"].ToString() +
                            "\",\"ibmid\":\"" + CRow[i]["ibmid"].ToString() + "\",\"csbfl\":\"" + CRow[i]["csbfl"].ToString() +
                            "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() +
                            "\",\"csbsbh\":\"" + CRow[i]["csbsbh"].ToString() + "\",\"cbmmc\":\"" + CRow[i]["cbmmc"].ToString() +
                            "\",\"cscx\":\"" + CRow[i]["cscx"].ToString() + "\",\"csszy\":\"" + CRow[i]["csszy"].ToString() +
                            "\",\"csbzt\":\"" + CRow[i]["csbzt"].ToString() + "\",\"csbdj\":\"" + CRow[i]["csbdj"].ToString() +
                            "\",\"csbxh\":\"" + CRow[i]["csbxh"].ToString() + "\",\"cjscs\":\"" + CRow[i]["cjscs"].ToString() +
                            "\",\"csccj\":\"" + CRow[i]["csccj"].ToString() + "\",\"izjsl\":\"" + CRow[i]["izjsl"].ToString() +
                            "\",\"cazdd\":\"" + CRow[i]["cazdd"].ToString() + "\",\"dtyrq\":\"" + CRow[i]["dtyrq"].ToString() +
                            "\",\"csdwz\":\"" + CRow[i]["csdwz"].ToString() + "\",\"cbz\":\"" + CRow[i]["cbz"].ToString() + "\"},");

                        }

                        else
                        {

                            //sb.Append("{\"id\":\"" + CRow[i]["id"].ToString() + "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() + "\"},");

                            sb.Append("{ \"pid\":\"" + CRow[i]["pid"].ToString() + "\",\"csjsb\":\"" + CRow[i]["csjsb"].ToString() +
                            "\",\"id\":\"" + CRow[i]["id"].ToString() + "\",\"ilbid\":\"" + CRow[i]["ilbid"].ToString() +
                            "\",\"iscxid\":\"" + CRow[i]["iscxid"].ToString() + "\",\"izyid\":\"" + CRow[i]["izyid"].ToString() +
                            "\",\"ibmid\":\"" + CRow[i]["ibmid"].ToString() + "\",\"csbfl\":\"" + CRow[i]["csbfl"].ToString() +
                            "\",\"csbbh\":\"" + CRow[i]["csbbh"].ToString() + "\",\"csbmc\":\"" + CRow[i]["csbmc"].ToString() +
                            "\",\"csbsbh\":\"" + CRow[i]["csbsbh"].ToString() + "\",\"cbmmc\":\"" + CRow[i]["cbmmc"].ToString() +
                            "\",\"cscx\":\"" + CRow[i]["cscx"].ToString() + "\",\"csszy\":\"" + CRow[i]["csszy"].ToString() +
                            "\",\"csbzt\":\"" + CRow[i]["csbzt"].ToString() + "\",\"csbdj\":\"" + CRow[i]["csbdj"].ToString() +
                            "\",\"csbxh\":\"" + CRow[i]["csbxh"].ToString() + "\",\"cjscs\":\"" + CRow[i]["cjscs"].ToString() +
                            "\",\"csccj\":\"" + CRow[i]["csccj"].ToString() + "\",\"izjsl\":\"" + CRow[i]["izjsl"].ToString() +
                            "\",\"cazdd\":\"" + CRow[i]["cazdd"].ToString() + "\",\"dtyrq\":\"" + CRow[i]["dtyrq"].ToString() +
                            "\",\"csdwz\":\"" + CRow[i]["csdwz"].ToString() + "\",\"cbz\":\"" + CRow[i]["cbz"].ToString() + "\"},");

                        }

                    }

                }

                sb.Replace(',', ' ', sb.Length - 1, 1);

                sb.Append("]},");

            }

            return sb.ToString();

        }


        //查询父项条件
        string str ="";

        public string GetStr(string sStr)
        {
            DataTable dTable = new DataTable();
            dTable = SqlHelper.GetTable("select pid,id from v_sbzl where " + sStr);
            if (dTable.Rows.Count > 0)
            {
                if (int.Parse (dTable.Rows[0]["pid"].ToString ()) != 0)
                {
                    str = str + " or " + "id=" + dTable.Rows[0]["pid"].ToString();
                    GetStr("id=" + dTable.Rows[0]["pid"].ToString ());
                }
            }

            return str;
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <returns></returns>
        public void Search(string lbid)
        {
            if (lbid == "1" || string.IsNullOrEmpty(lbid))
            {
                lbid = "1";
            }

            StringBuilder sb = new StringBuilder("");

            string value = HttpContext.Current.Request["data"];
            string[] strr = value.Split('&');

            string s_where = "1=1";
            string s_csbbh = strr[0].Split('=')[1];
            if (s_csbbh.Trim().Length > 0)
            {
                s_where = s_where + " and csbbh  like '%" + s_csbbh + "%'";
            }

            string s_csbmc = strr[1].Split('=')[1];
            if (s_csbmc.Trim().Length > 0)
            {
                s_where = s_where + " and csbmc like '%" + s_csbmc + "%'";
            }

            string s_csbsbh = strr[2].Split('=')[1];
            if (s_csbsbh.Trim().Length > 0)
            {
                s_where = s_where + " and csbsbh like '%" + s_csbsbh + "%'";
            }

            string s_csszy = strr[3].Split('=')[1];
            if (s_csszy.Trim().Length > 0)
            {
                s_where = s_where + " and izyid='" + s_csszy + "'";
            }

            string s_cscx = strr[4].Split('=')[1];
            if (s_cscx.Trim().Length > 0)
            {
                s_where = s_where + " and iscxid = '" + s_cscx + "'";
            }

            string s_csbxh = strr[5].Split('=')[1];
            if (s_csbxh.Trim().Length > 0)
            {
                s_where = s_where + " and csbxh like '%" + s_csbxh + "%'";
            }

            string s_csdwz = strr[6].Split('=')[1];
            if (s_csdwz.Trim().Length > 0)
            {
                s_where = s_where + " and csdwz like '%" + s_csdwz + "%'";
            }

            string s_cjscs = strr[7].Split('=')[1];
            if (s_cjscs.Trim().Length > 0)
            {
                s_where = s_where + " and cjscs like '%" + s_cjscs + "%'";
            }

            string s_csbzt = strr[8].Split('=')[1];
            if (s_csbzt.Trim().Length > 0)
            {
                s_where = s_where + " and csbzt like '%" + s_csbzt + "%'";
            }

            string s_csbdj = strr[9].Split('=')[1];
            if (s_csbdj.Trim().Length > 0)
            {
                s_where = s_where + " and csbdj like '%" + s_csbdj + "%'";
            }

            string s_csccj = strr[10].Split('=')[1];
            if (s_csccj.Trim().Length > 0)
            {
                s_where = s_where + " and csccj like '%" + s_csccj + "%'";
            }

            string s_izjsl = strr[11].Split('=')[1];
            if (s_izjsl.Trim().Length > 0)
            {
                s_where = s_where + " and izjsl like '%" + s_izjsl + "%'";
            }

            string s_cazdd = strr[12].Split('=')[1];
            if (s_cazdd.Trim().Length > 0)
            {
                s_where = s_where + " and cazdd like '%" + s_cazdd + "%'";
            }

            string s_cbz = strr[13].Split('=')[1];
            if (s_cbz.Trim().Length > 0)
            {
                s_where = s_where + " and cbz like '%" + s_cbz + "%'";
            }

            //使用存储过程及临时表实现递归方式列表
            SqlParameter[] parms = {
                            new SqlParameter("@id",lbid)
                                 };
            SqlHelper.ExeNonQuery("get_sbfl2", CommandType.StoredProcedure, parms);

            string strWhere = s_where + GetStr(s_where);

            DataTable dt = new DataTable();
            dt = SqlHelper.GetTable("select * from v_sbzl where " + strWhere + " order by pid,csbfl,csbbh asc");

            if (dt.Rows.Count > 0)
            {

                sb.Append(GetDataString(dt, "0"));


                sb = sb.Remove(sb.Length - 2, 2);

            }

            HttpContext.Current.Response.Write(sb.ToString());
        }


        /// <summary>
        /// 组合搜索条件
        /// </summary>
        /// <returns></returns>
        private string GetWhere()
        {
            StringBuilder sb = new StringBuilder("");
            string searchType = HttpContext.Current.Request["search_type"] != "" ? HttpContext.Current.Request["search_type"] : string.Empty;
            string searchValue = HttpContext.Current.Request["search_value"] != "" ? HttpContext.Current.Request["search_value"] : string.Empty;
            if (searchType != null && searchValue != null)
            {
                str = searchType + " like '%" + searchValue + "%'";
                sb.Append(GetStr(str));
                //sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            else
            {
                sb.Append("1=1");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string lbid)
        {
            try
            {
                string strWhere = GetWhere();
                if (lbid == "1" || string.IsNullOrEmpty(lbid))
                {
                    lbid = "1";
                }

                //使用存储过程及临时表实现递归方式列表
                SqlParameter[] parms = {
                            new SqlParameter("@id",lbid)
                                 };
                SqlHelper.ExeNonQuery("get_sbfl2", CommandType.StoredProcedure, parms);

                StringBuilder sb = new StringBuilder("");
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select * from v_sbzl where " + strWhere + " order by pid,csbfl,csbbh asc");

                if (dt.Rows.Count > 0)
                {

                    sb.Append(GetDataString(dt, "0"));


                    sb = sb.Remove(sb.Length - 2, 2);

                }

                HttpContext.Current.Response.Write(sb.ToString());
            }
            catch(Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        //删除的方法
        private void Del()
        {
            try
            {
                //获取到选中行的id
                string id = HttpContext.Current.Request["id"];

                string sSql = "";
                DataTable dt = new DataTable();

                //判断维修记录中是否存在
                sSql = "select * from wxjlb where isbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                //判断保养记录中是否存在
                sSql = "select * from zybyb where isbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                //判断点检记录中是否存在
                sSql = "select * from djjlb where isbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                //判断信号强制中是否存在
                sSql = "select * from xhqzb where isbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("4");
                    return;
                }

                //判断停机记录中是否存在
                sSql = "select * from tjjlb where isbid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("5");
                    return;
                }

                string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
                SqlConnection con = new SqlConnection(strConn);
                con.Open();

                SqlTransaction sqltra = con.BeginTransaction();//开始事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;//获取数据连接
                cmd.Transaction = sqltra;//，在执行SQL时

                string sql = "";

                ////查找备件编号
                ////注意：该段查找备件编号代码需放在删除备件之前，否则容易发生timeout错误
                ////原因是在删除备件后，sbzlb_bjmx表已经加了独占锁，所以放在删除之前查找。
                ////---------------------------------------------------------
                //string cbjbh = "";

                //sql = "select b.cbjbh from sbzlb s inner join sbzlb_bjmx sm on s.id=sm.isbid " +
                //        " inner join sbbjb b on sm.ibjid=b.id " +
                //            " where s.id='" + id + "'";
                //dt = SqlHelper.GetTable(sql);
                //if (dt.Rows.Count > 0)
                //{
                //    cbjbh = dt.Rows[0]["cbjbh"].ToString();
                //}
                //dt = null;      //释放表
                ////------------------------------------------------------------

                //备件
                sql = "delete from sbzlb_bjmx where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //部件
                sql = "delete from sbzlb_bjmx2 where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //保养
                sql = "delete from sbzlb_by where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //文档
                sql = "delete from wdglb where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //点检方式
                sql = "delete from sbzlb_djfs where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //点检参数
                sql = "delete from sbzlb_djcs where isbid=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //设备资料
                sql = "delete from sbzlb where id=" + id;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //SqlParameter[] parms_sbbj = {
                //                new SqlParameter("@bjbh",cbjbh)
                //            };

                SqlCommand cmds = new SqlCommand();
                cmds.Connection = con;//获取数据连接
                cmds.Transaction = sqltra;//，在执行SQL时
                cmds.CommandText = "update_sysb";
                cmds.CommandType = CommandType.StoredProcedure;
                cmds.ExecuteNonQuery();

                sqltra.Commit();        //提交事务

                if (con.State == ConnectionState.Open) con.Close();
                con.Dispose();

                HttpContext.Current.Response.Write("删除成功！");
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }
        //添加
        private void Add()
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
            string[] str = ss.Split('~');
            string[] strSb = str[0].Split('&');     //设备资料
            int rCount = 0;     //总行数
            string[] value;

            DataTable dt = new DataTable();

            //查找id最大值
            Int64 id = 0;
            dt = SqlHelper.GetTable("select max(id) as maxid from sbzlb");
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["maxid"].ToString()))
                {
                    id = Int64.Parse(dt.Rows[0]["maxid"].ToString()) + 1;
                }
                else
                {
                    id = 1;
                }
            }
            else
            {
                id = 1;
            }
            dt = null;

            sys ex = new sys();

            if (strSb[2].Split('=')[1] == "")
            {
                HttpContext.Current.Response.Write("A");
                return;
            }
            int sjid = 0;
            if (ex.isNumber(strSb[1].Split('=')[1])) sjid = int.Parse(strSb[1].Split('=')[1]);
            int ilbid = int.Parse(strSb[2].Split('=')[1]);
            string DeviceId = strSb[3].Split('=')[1];
            string DeviceName = strSb[4].Split('=')[1];
            string csbsbh = strSb[5].Split('=')[1];
            string ibmid = strSb[6].Split('=')[1];
            string belong = strSb[7].Split('=')[1];
            string line = strSb[8].Split('=')[1];
            string csbxh = strSb[9].Split('=')[1];
            string csdwz = strSb[10].Split('=')[1];

            
            string csbzt = strSb[11].Split('=')[1];
            string csbdj = strSb[12].Split('=')[1];
            string csccj = strSb[13].Split('=')[1];

            double izjsl = 0;
            if (ex.isNumber(strSb[14].Split('=')[1]) == true)
            {
                izjsl = double.Parse(strSb[14].Split('=')[1]);
            }
            string cazdd = strSb[15].Split('=')[1];
            string dtyrq = "";
            if (ex.isDate(strSb[16].Split('=')[1]) == true)
            {
                dtyrq = strSb[16].Split('=')[1];
            }

            string csjsb = strSb[17].Split('=')[1];
            string cjscs = strSb[18].Split('=')[1];
            string cbz = strSb[19].Split('=')[1];

            //设备编号不允许为空
            if (DeviceId.Length == 0)
            {
                HttpContext.Current.Response.Write("B");
                return;
            }
            //设备名称不允许为空
            if (DeviceName.Length == 0)
            {
                HttpContext.Current.Response.Write("C");
                return;
            }

            //判断设备编号是否存在
            dt = SqlHelper.GetTable("select * from sbzlb where DeviceId='" + DeviceId + "'");
            if (dt.Rows.Count > 0)
            {
                HttpContext.Current.Response.Write("D");
                return;
            }

            ////rfid不允许为空
            //if (csbsbh.Length == 0)
            //{
            //    HttpContext.Current.Response.Write("U");
            //    return;
            //}

            //判断RFID是否存在
            if (csbsbh.Trim().Length > 0)
            {
                dt = SqlHelper.GetTable("select * from sbzlb where csbsbh='" + csbsbh + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("V");
                    return;
                }
            }

            //判断上级设备是否与设备编号重复
            if (csjsb.Trim().Length > 0)
            {
                if (csjsb.Trim() == DeviceId.Trim())
                {
                    HttpContext.Current.Response.Write("X");
                    return;
                }
            }

            //判断上级设备编号是否存在
            if (csjsb.Trim().Length > 0)
            {
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csjsb + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("Y");
                    return;
                }
            }
            else
            {
                sjid = 0;       //上级设备为空
            }

            string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
            SqlConnection con = new SqlConnection(strConn);
            con.Open();

            SqlTransaction sqltra = con.BeginTransaction();//开始事务
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;//获取数据连接
            cmd.Transaction = sqltra;//，在执行SQL时

            SqlParameter[] parms = {
                            new SqlParameter("@id",id),  
                            new SqlParameter("@sjid",sjid),              
                            new SqlParameter("@lbid",ilbid),
                            new SqlParameter("@sbbh",DeviceId),
                            new SqlParameter("@sbmc",DeviceName),
                            new SqlParameter("@sbsbh",csbsbh),
                            new SqlParameter("@scx",line),
                            new SqlParameter("@bm",ibmid),
                            new SqlParameter("@sszy",belong),
                            new SqlParameter("@sdwz",csdwz),
                            new SqlParameter("@sbxh",csbxh),
                            new SqlParameter("@jscs",cjscs),
                            new SqlParameter("@sccj",csccj),
                            new SqlParameter("@zjsl",izjsl),
                            new SqlParameter("@azdd",cazdd),
                            new SqlParameter("@tyrq",dtyrq),
                            new SqlParameter("@sbdj",csbdj),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@sbzt",csbzt)
                                 };

            cmd.CommandText = "insert_sbzl";
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in parms)
            {
                cmd.Parameters.Add(p);
            }
            cmd.ExecuteNonQuery();

            string strSql = "";
            DataTable dtMx = new DataTable();

            //备件资料
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[1].Split('*')[2]) || str[1].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_bj = {
                            new SqlParameter("@sbid",id)
                        };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "del_sbzl_sbbj";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_bj)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                rCount = int.Parse(str[1].Split('*')[2]);
                string[] strBj = str[1].Split('*')[1].Split('@');
                string bjbh = "";
                double sl = 0;

                //备件资料
                for (int i = 0; i < rCount; i++)
                {
                    value = strBj[i].Split('&');
                    bjbh = value[1];

                    if (bjbh.Trim() == "" || string.IsNullOrEmpty(bjbh))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("E&" + (i + 1));
                        return;
                    }

                    ////判断部件编号是否存在
                    //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("F&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    if (value[2].Trim() == "" || string.IsNullOrEmpty(value[2]))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("G&" + (i + 1));
                        return;
                    }

                    //sl = double.Parse(value[2]);

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bjbh",bjbh),
                            new SqlParameter("@bjsl",sl)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbbj";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();


                    SqlParameter[] parms_sbbj = {
                            new SqlParameter("@sbbh",DeviceId),
                            new SqlParameter("@bjbh",bjbh)
                        };

                    SqlCommand cmds = new SqlCommand();
                    cmds.Connection = con;//获取数据连接
                    cmds.Transaction = sqltra;//，在执行SQL时
                    cmds.CommandText = "insert_sysb";
                    cmds.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_sbbj)
                    {
                        cmds.Parameters.Add(p);
                    }
                    cmds.ExecuteNonQuery();
                }
            }
            //--------------------------------------------------------------


            //部件资料
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[2].Split('*')[2]) || str[2].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_bj2 = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_bj2 = new SqlCommand();
                cmd_bj2.Connection = con;//获取数据连接
                cmd_bj2.Transaction = sqltra;//，在执行SQL时
                cmd_bj2.CommandText = "del_sbzl_sbbj2";
                cmd_bj2.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_bj2)
                {
                    cmd_bj2.Parameters.Add(p);
                }
                cmd_bj2.ExecuteNonQuery();

                rCount = int.Parse(str[2].Split('*')[2]);
                string[] strBj2 = str[2].Split('*')[1].Split('@');
                string bjbh2 = "";
                string bjmc2 = "";
                string bjsbh = "";
                int bmid = 0;
                int zyid = 0;
                int scxid = 0;
                string bjxh = "";
                string jscs = "";
                string bjzt = "";
                string bjdj = "";
                string sccj = "";
                double zjsl = 0;
                string bz = "";

                for (int i = 0; i < rCount; i++)
                {
                    value = strBj2[i].Split('&');
                    bjbh2 = value[1];
                    bjmc2 = value[2];
                    bjsbh = value[3];
                    bjzt = value[4];
                    bjdj = value[5];
                    if (ex.isNumber(value[6]) == true) bmid = int.Parse(value[6]);
                    if (ex.isNumber(value[7]) == true) zyid = int.Parse(value[7]);
                    if (ex.isNumber(value[8]) == true) scxid = int.Parse(value[8]);
                    bjxh = value[9];
                    jscs = value[10];
                    sccj = value[11];
                    if (ex.isNumber(value[12]) == true)
                    {
                        zjsl = double.Parse(value[12]);
                    }
                    bz = value[13];

                    if (bjbh2.Trim() == "" || string.IsNullOrEmpty(bjbh2))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("H&" + (i + 1));
                        return;
                    }

                    //判断部件编号是否存在
                    strSql = "select * from sbzlb_bjmx2 where cbjbh='" + bjbh2 + "'";
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = strSql;
                    comm.CommandType = CommandType.Text;
                    object srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("I&" + (i + 1));
                        return;
                    }

                    if (bjmc2.Trim() == "" || string.IsNullOrEmpty(bjmc2))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("J&" + (i + 1));
                        return;
                    }

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bjbh",bjbh2),
                            new SqlParameter("@bjmc",bjmc2),
                            new SqlParameter("@bjsbh",bjsbh),
                            new SqlParameter("@bjzt",bjzt),
                            new SqlParameter("@bjdj",bjdj),
                            new SqlParameter("@bm",bmid),
                            new SqlParameter("@sszy",zyid),
                            new SqlParameter("@scx",scxid),
                            new SqlParameter("@bjxh",bjxh),
                            new SqlParameter("@jscs",jscs),
                            new SqlParameter("@sccj",sccj),
                            new SqlParameter("@zjsl",zjsl),
                            new SqlParameter("@bz",bz)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbbj2";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();

                }
            }
            //--------------------------------------------------------------


            //保养
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[3].Split('*')[2]) || str[3].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_by = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_by = new SqlCommand();
                cmd_by.Connection = con;//获取数据连接
                cmd_by.Transaction = sqltra;//，在执行SQL时
                cmd_by.CommandText = "del_sbzl_by";
                cmd_by.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_by)
                {
                    cmd_by.Parameters.Add(p);
                }
                cmd_by.ExecuteNonQuery();

                rCount = int.Parse(str[3].Split('*')[2]);
                string[] strBy = str[3].Split('*')[1].Split('@');
                string bylx = "";
                double byzq = 0;
                string bydw = "";
                string byr = "";

                for (int i = 0; i < rCount; i++)
                {
                    value = strBy[i].Split('&');
                    bylx = value[1];

                    if (bylx.Trim() == "" || string.IsNullOrEmpty(bylx))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("K&" + (i + 1));
                        return;
                    }

                    ////判断保养类型是否存在
                    //strSql = "select * from bylxb where id=" + bylx ;
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("L&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    bydw = value[2];
                    if (value[2].Trim() == "" || string.IsNullOrEmpty(value[2]))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("M&" + (i + 1));
                        return;
                    }

                    if (value[3].Trim() == "" || string.IsNullOrEmpty(value[3]))      //保养周期
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("N&" + (i + 1));
                        return;
                    }
                    else
                    {
                        byzq = double.Parse(value[3]);
                    }

                    byr = value[4];     //保养人


                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bylx",bylx),
                            new SqlParameter("@bydw",bydw),
                            new SqlParameter("@byzq",byzq),
                            new SqlParameter("@byr",byr),
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_by";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();

                }
            }
            //--------------------------------------------------------------

            //文档
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[4].Split('*')[2]) || str[4].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_wd = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_wd = new SqlCommand();
                cmd_wd.Connection = con;//获取数据连接
                cmd_wd.Transaction = sqltra;//，在执行SQL时
                cmd_wd.CommandText = "del_sbzl_wdgl";
                cmd_wd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_wd)
                {
                    cmd_wd.Parameters.Add(p);
                }
                cmd_wd.ExecuteNonQuery();

                rCount = int.Parse(str[4].Split('*')[2]);
                string[] strWd = str[4].Split('*')[1].Split('@');
                string wd = "";

                for (int i = 0; i < rCount; i++)
                {
                    value = strWd[i].Split('&');
                    wd = value[1];

                    if (wd.Trim() == "" || string.IsNullOrEmpty(wd))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("O&" + (i + 1));
                        return;
                    }

                    ////判断文档是否存在
                    //strSql = "select * from wdglb where cnr='" + wd + "'";
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count > 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("P&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;


                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@nr",wd)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_wdgl";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();

                }
                //--------------------------------------------------------------


                //点检
                //--------------------------------------------------------------
                if (!string.IsNullOrEmpty(str[5].Split('*')[2]) || str[5].Split('*')[2].ToString() != "0")
                {
                    SqlParameter[] pm_dj = {
                            new SqlParameter("@sbid",id)
                        };

                    SqlCommand cmd_dj = new SqlCommand();
                    cmd_dj.Connection = con;//获取数据连接
                    cmd_dj.Transaction = sqltra;//，在执行SQL时
                    cmd_dj.CommandText = "del_sbzl_sbdj";
                    cmd_dj.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in pm_dj)
                    {
                        cmd_dj.Parameters.Add(p);
                    }
                    cmd_dj.ExecuteNonQuery();

                    rCount = int.Parse(str[5].Split('*')[2]);
                    string[] strDj = str[5].Split('*')[1].Split('@');
                    string djfs = "";       //点检方式
                    string djdw = "";       //点检单位
                    double djzq = 0;        //点检周期
                    string djr = "";        //点检人
                    string bjr = "";        //报警人

                    for (int i = 0; i < rCount; i++)
                    {
                        value = strDj[i].Split('&');
                        djfs = value[1];

                        if (djfs.Trim() == "" || string.IsNullOrEmpty(djfs))
                        {
                            con.Close();
                            HttpContext.Current.Response.Write("Q&" + (i + 1));
                            return;
                        }

                        ////判断点检方式是否存在
                        //strSql = "select * from CabinetRecordType where id=" + djfs;
                        //dtMx = SqlHelper.GetTable(strSql);
                        //if (dtMx.Rows.Count == 0)
                        //{
                        //    dtMx = null;      //出错后释放
                        //    con.Close();

                        //    HttpContext.Current.Response.Write("R&" + (i + 1));
                        //    return;
                        //}
                        //dtMx = null;

                        djdw = value[2];

                        if (ex.isNumber(value[3]) == true)
                        {
                            djzq = double.Parse(value[3]);
                        }
                        djr = value[4];
                        bjr = value[5];


                        SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@djid",djfs),
                            new SqlParameter("@djdw",djdw),
                            new SqlParameter("@djzq",djzq),
                            new SqlParameter("@djr",djr),
                            new SqlParameter("@bjr",bjr)
                        };

                        SqlCommand cmd_mx = new SqlCommand();
                        cmd_mx.Connection = con;//获取数据连接
                        cmd_mx.Transaction = sqltra;//，在执行SQL时
                        cmd_mx.CommandText = "insert_sbzl_sbdj";
                        cmd_mx.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter p in parms_mx)
                        {
                            cmd_mx.Parameters.Add(p);
                        }
                        cmd_mx.ExecuteNonQuery();

                    }
                }
            }
            //--------------------------------------------------------------

            //点检参数
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[6].Split('*')[2]) || str[6].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_djcs = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_djcs = new SqlCommand();
                cmd_djcs.Connection = con;//获取数据连接
                cmd_djcs.Transaction = sqltra;//，在执行SQL时
                cmd_djcs.CommandText = "del_sbzl_djcs";
                cmd_djcs.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_djcs)
                {
                    cmd_djcs.Parameters.Add(p);
                }
                cmd_djcs.ExecuteNonQuery();

                rCount = int.Parse(str[6].Split('*')[2]);
                string[] strDjcs = str[6].Split('*')[1].Split('@');
                string djcs = "";       //点检参数
                double xx = 0;       //下限
                double sx = 0;        //上限  
                string blm = "";       //变量名
                string inumber = "";        //对应点检方式表中的inumber字段

                for (int i = 0; i < rCount; i++)
                {
                    value = strDjcs[i].Split('&');
                    djcs = value[1];

                    if (djcs.Trim() == "" || string.IsNullOrEmpty(djcs))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("S&" + (i + 1));
                        return;
                    }

                    ////判断点检参数是否存在
                    //strSql = "select * from CabinetRecordType where inumber1='" + djcs + "' or inumber2='" + djcs + "' or inumber3='" + djcs + "' or inumber4='" + djcs + "' or inumber5='" + djcs + "' or inumber6='" + djcs + "' or inumber7='" + djcs + "' or inumber8='" + djcs + "' or inumber9='" + djcs + "' or inumber10='" + djcs + "'";
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("T&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    if (ex.isNumber(value[2]) == true)
                    {
                        xx = double.Parse(value[2]);
                    }
                    if (ex.isNumber(value[3]) == true)
                    {
                        sx = double.Parse(value[3]);
                    }

                    blm = value[4];

                    //if (blm.Trim() == "" || string.IsNullOrEmpty(blm))
                    //{
                    //    con.Close();
                    //    HttpContext.Current.Response.Write("W&" + (i + 1));
                    //    return;
                    //}

                    inumber = value[5].Trim ();

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@djcs",djcs),
                            new SqlParameter("@inumber",inumber),
                            new SqlParameter("@xx",xx),
                            new SqlParameter("@sx",sx),
                            new SqlParameter("@blm",blm)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_djcs";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();

                }
            }
            //--------------------------------------------------------------


            sqltra.Commit();        //提交事务

            if (con.State == ConnectionState.Open) con.Close();
            con.Dispose();

            HttpContext.Current.Response.Write("添加成功！");
            }
            catch
            {
                HttpContext.Current.Response.Write("添加失败！");
            }

        }

        private void Edit()
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
            string[] str = ss.Split('~');
            string[] strSb = str[0].Split('&');     //设备资料
            int rCount = 0;     //总行数
            string[] value;

            DataTable dt = new DataTable();

            //id
            int id = int.Parse(strSb[0].Split('=')[1]);

            if (strSb[2].Split('=')[1] == "")
            {
                HttpContext.Current.Response.Write("A");
                return;
            }

            sys ex = new sys();

            int sjid = 0;
            if (ex.isNumber(strSb[1].Split('=')[1])) sjid = int.Parse(strSb[1].Split('=')[1]);
            int ilbid = int.Parse(strSb[2].Split('=')[1]);
            string DeviceId = strSb[3].Split('=')[1];
            string DeviceName = strSb[4].Split('=')[1];
            string csbsbh = strSb[5].Split('=')[1];
            string ibmid = strSb[6].Split('=')[1];
            string belong = strSb[7].Split('=')[1];
            string line = strSb[8].Split('=')[1];
            string csbxh = strSb[9].Split('=')[1];
            string csdwz = strSb[10].Split('=')[1];

            
            string csbzt = strSb[11].Split('=')[1];
            string csbdj = strSb[12].Split('=')[1];
            string csccj = strSb[13].Split('=')[1];

            double izjsl = 0;
            if (ex.isNumber(strSb[14].Split('=')[1]) == true)
            {
                izjsl = double.Parse(strSb[14].Split('=')[1]);
            }
            string cazdd = strSb[15].Split('=')[1];
            string dtyrq = "";
            if (ex.isDate(strSb[16].Split('=')[1]) == true)
            {
                dtyrq = strSb[16].Split('=')[1];
            }

            string csjsb = strSb[17].Split('=')[1];
            string cjscs = strSb[18].Split('=')[1];
            string cbz = strSb[19].Split('=')[1];

            //设备编号不允许为空
            if (DeviceId.Length == 0)
            {
                HttpContext.Current.Response.Write("B");
                return;
            }
            //设备名称不允许为空
            if (DeviceName.Length == 0)
            {
                HttpContext.Current.Response.Write("C");
                return;
            }

            //判断设备编号是否存在
            dt = SqlHelper.GetTable("select * from sbzlb where DeviceId='" + DeviceId + "' and id<>" + id);
            if (dt.Rows.Count > 0)
            {
                HttpContext.Current.Response.Write("D");
                return;
            }
            dt = null;

            ////rfid不允许为空
            //if (csbsbh.Length == 0)
            //{
            //    HttpContext.Current.Response.Write("U");
            //    return;
            //}

            //判断RFID是否存在
            if (csbsbh.Trim().Length > 0)
            {
                dt = SqlHelper.GetTable("select * from sbzlb where csbsbh='" + csbsbh + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("V");
                    return;
                }
            }

            //判断上级设备是否与设备编号重复
            if (csjsb.Trim().Length > 0)
            {
                if (csjsb.Trim() == DeviceId.Trim())
                {
                    HttpContext.Current.Response.Write("X");
                    return;
                }
            }

            //判断上级设备编号是否存在
            if (csjsb.Trim().Length > 0)
            {
                dt = SqlHelper.GetTable("select * from sbzlb where deviceid='" + csjsb + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("Y");
                    return;
                }
            }
            else
            {
                sjid = 0;       //上级设备为空
            }

            string strConn = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
            SqlConnection con = new SqlConnection(strConn);
            con.Open();

            SqlTransaction sqltra = con.BeginTransaction();//开始事务
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;//获取数据连接
            cmd.Transaction = sqltra;//，在执行SQL时

            SqlParameter[] parms = {
                            new SqlParameter("@id",id),   
                            new SqlParameter("@sjid",sjid),             
                            new SqlParameter("@lbid",ilbid),
                            new SqlParameter("@sbbh",DeviceId),
                            new SqlParameter("@sbmc",DeviceName),
                            new SqlParameter("@sbsbh",csbsbh),
                            new SqlParameter("@scx",line),
                            new SqlParameter("@bm",ibmid),
                            new SqlParameter("@sszy",belong),
                            new SqlParameter("@sdwz",csdwz),
                            new SqlParameter("@sbxh",csbxh),
                            new SqlParameter("@jscs",cjscs),
                            new SqlParameter("@sccj",csccj),
                            new SqlParameter("@zjsl",izjsl),
                            new SqlParameter("@azdd",cazdd),
                            new SqlParameter("@tyrq",dtyrq),
                            new SqlParameter("@sbdj",csbdj),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@sbzt",csbzt)
                                 };

            cmd.CommandText = "update_sbzl";
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in parms)
            {
                cmd.Parameters.Add(p);
            }
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            string strSql = "";
            DataTable dtMx = new DataTable();

            //备件资料
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[1].Split('*')[2]) || str[1].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_bj = {
                            new SqlParameter("@sbid",id)
                        };

                SqlCommand com = new SqlCommand();
                com.Connection = con;//获取数据连接
                com.Transaction = sqltra;//，在执行SQL时
                com.CommandText = "del_sbzl_sbbj";
                com.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_bj)
                {
                    com.Parameters.Add(p);
                }
                com.ExecuteNonQuery();

                //插入新的设备明细
                rCount = int.Parse(str[1].Split('*')[2]);
                string[] strBj = str[1].Split('*')[1].Split('@');
                string bjbh = "";
                double sl = 0;

                //备件资料
                for (int i = 0; i < rCount; i++)
                {
                    value = strBj[i].Split('&');
                    bjbh = value[1];

                    if (bjbh.Trim() == "" || string.IsNullOrEmpty(bjbh))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("E&" + (i + 1));
                        return;
                    }

                    ////判断部件编号是否存在
                    //strSql = "select * from sbbjb where cbjbh='" + bjbh + "'";
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("F&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    if (value[2].Trim() == "" || string.IsNullOrEmpty(value[2]))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("G&" + (i + 1));
                        return;
                    }

                    //sl = double.Parse(value[2]);

                    //插入新的备件资料
                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bjbh",bjbh),
                            new SqlParameter("@bjsl",sl)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbbj";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();

                }

                //更新适用设备
                SqlCommand cmds = new SqlCommand();
                cmds.Connection = con;//获取数据连接
                cmds.Transaction = sqltra;//，在执行SQL时
                cmds.CommandText = "update_sysb";
                cmds.CommandType = CommandType.StoredProcedure;
                cmds.ExecuteNonQuery();
                cmds.Dispose();
            }
            //--------------------------------------------------------------


            //部件资料
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[2].Split('*')[2]) || str[2].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_bj2 = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_bj2 = new SqlCommand();
                cmd_bj2.Connection = con;//获取数据连接
                cmd_bj2.Transaction = sqltra;//，在执行SQL时
                cmd_bj2.CommandText = "del_sbzl_sbbj2";
                cmd_bj2.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_bj2)
                {
                    cmd_bj2.Parameters.Add(p);
                }
                cmd_bj2.ExecuteNonQuery();

                //插入新的部件明细
                rCount = int.Parse(str[2].Split('*')[2]);
                string[] strBj2 = str[2].Split('*')[1].Split('@');
                int bjid2 = 0;
                string bjbh2 = "";
                string bjmc2 = "";
                string bjsbh = "";
                int bmid = 0;
                int zyid = 0;
                int scxid = 0;
                string bjxh = "";
                string jscs = "";
                string bjzt = "";
                string bjdj = "";
                string sccj = "";
                double zjsl = 0;
                string bz = "";

                for (int i = 0; i < rCount; i++)
                {
                    value = strBj2[i].Split('&');

                    if (ex.isNumber(value[1]) == true)
                    {
                        bjid2 = int.Parse(value[1]);
                    }

                    bjbh2 = value[2];
                    bjmc2 = value[3];
                    bjsbh = value[4];
                    bjzt = value[5];
                    bjdj = value[6];

                    if (ex.isNumber(value[7]) == true) bmid = int.Parse(value[7]);
                    if (ex.isNumber(value[8]) == true) zyid = int.Parse(value[8]);
                    if (ex.isNumber(value[9]) == true) scxid = int.Parse(value[9]);
                    bjxh = value[10];
                    jscs = value[11];
                    sccj = value[12];
                    if (ex.isNumber(value[13]) == true)
                    {
                        zjsl = double.Parse(value[13]);
                    }
                    bz = value[14];

                    if (bjbh2.Trim() == "" || string.IsNullOrEmpty(bjbh2))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("H&" + (i + 1));
                        return;
                    }

                    //部件编号是否存在
                    strSql = "select * from sbzlb_bjmx2 where cbjbh='" + bjbh2 + "'";
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = con;
                    comm.Transaction = sqltra;//，在执行SQL时
                    comm.CommandText = strSql;
                    comm.CommandType = CommandType.Text;
                    object srows = comm.ExecuteScalar();        //如果采用强制类型转换，如comm.ExecuteScalar().tostring(),容易出现未将对象引用设置到对象的实例
                    if (srows != null)
                    {
                        comm.Dispose();
                        con.Close();
                        con.Dispose();

                        HttpContext.Current.Response.Write("I&" + (i + 1));
                        return;
                    }

                    if (bjmc2.Trim() == "")
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("J&" + (i + 1));
                        return;
                    }

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bjbh",bjbh2),
                            new SqlParameter("@bjmc",bjmc2),
                            new SqlParameter("@bjsbh",bjsbh),
                            new SqlParameter("@bjzt",bjzt),
                            new SqlParameter("@bjdj",bjdj),
                            new SqlParameter("@bm",bmid),
                            new SqlParameter("@sszy",zyid),
                            new SqlParameter("@scx",scxid),
                            new SqlParameter("@bjxh",bjxh),
                            new SqlParameter("@jscs",jscs),
                            new SqlParameter("@sccj",sccj),
                            new SqlParameter("@zjsl",zjsl),
                            new SqlParameter("@bz",bz)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbbj2";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();
                }
            }
            //--------------------------------------------------------------


            //保养
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[3].Split('*')[2]) || str[3].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_by = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_by = new SqlCommand();
                cmd_by.Connection = con;//获取数据连接
                cmd_by.Transaction = sqltra;//，在执行SQL时
                cmd_by.CommandText = "del_sbzl_by";
                cmd_by.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_by)
                {
                    cmd_by.Parameters.Add(p);
                }
                cmd_by.ExecuteNonQuery();

                //插入新的保养明细
                rCount = int.Parse(str[3].Split('*')[2]);
                string[] strBy = str[3].Split('*')[1].Split('@');
                string bylx = "";
                double byzq = 0;
                string bydw = "";
                string byr = "";

                for (int i = 0; i < rCount; i++)
                {
                    value = strBy[i].Split('&');
                    bylx = value[1];

                    if (bylx.Trim() == "" || string.IsNullOrEmpty(bylx))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("K&" + (i + 1));
                        return;
                    }

                    ////判断保养类型是否存在
                    //strSql = "select * from bylxb where id=" + bylx;
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("L&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    bydw = value[2];
                    if (value[2].Trim() == "" || string.IsNullOrEmpty(value[2]))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("M&" + (i + 1));
                        return;
                    }

                    if (value[3].Trim() == "" || string.IsNullOrEmpty(value[3]))      //保养周期
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("N&" + (i + 1));
                        return;
                    }
                    else
                    {
                        byzq = double.Parse(value[3]);
                    }

                    byr = value[4];     //保养人


                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@bylx",bylx),
                            new SqlParameter("@bydw",bydw),
                            new SqlParameter("@byzq",byzq),
                            new SqlParameter("@byr",byr),
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_by";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();
                }
            }
            //--------------------------------------------------------------

            //文档
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[4].Split('*')[2]) || str[4].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_wd = {
                            new SqlParameter("@sbid",id)
                        };

                SqlCommand cmd_wd = new SqlCommand();
                cmd_wd.Connection = con;//获取数据连接
                cmd_wd.Transaction = sqltra;//，在执行SQL时
                cmd_wd.CommandText = "del_sbzl_wdgl";
                cmd_wd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_wd)
                {
                    cmd_wd.Parameters.Add(p);
                }
                cmd_wd.ExecuteNonQuery();

                //插入新的文档明细
                rCount = int.Parse(str[4].Split('*')[2]);
                string[] strWd = str[4].Split('*')[1].Split('@');
                string wd = "";
                int wdid = 0;

                for (int i = 0; i < rCount; i++)
                {
                    value = strWd[i].Split('&');

                    if (ex.isNumber(value[1]) == true)
                    {
                        wdid = int.Parse(value[1]);
                    }

                    wd = value[2];

                    if (wd.Trim() == "" || string.IsNullOrEmpty(wd))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("O&" + (i + 1));
                        return;
                    }

                    ////判断文档是否存在
                    //strSql = "select * from wdglb where cnr='" + wd + "' and id<>" + wdid;
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count > 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("P&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;


                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@nr",wd)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_wdgl";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();
                }
            }
            //--------------------------------------------------------------


            //点检
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[5].Split('*')[2]) || str[5].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_dj = {
                            new SqlParameter("@sbid",id)
                        };

                SqlCommand cmd_dj = new SqlCommand();
                cmd_dj.Connection = con;//获取数据连接
                cmd_dj.Transaction = sqltra;//，在执行SQL时
                cmd_dj.CommandText = "del_sbzl_sbdj";
                cmd_dj.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_dj)
                {
                    cmd_dj.Parameters.Add(p);
                }
                cmd_dj.ExecuteNonQuery();
                cmd_dj.Dispose();

                //插入新的点检明细
                rCount = int.Parse(str[5].Split('*')[2]);
                string[] strDj = str[5].Split('*')[1].Split('@');
                string djfs = "";       //点检方式
                string djdw = "";       //点检单位
                double djzq = 0;        //点检周期
                string djr = "";        //点检人
                string bjr = "";        //报警人

                for (int i = 0; i < rCount; i++)
                {
                    value = strDj[i].Split('&');
                    djfs = value[1];

                    if (djfs.Trim() == "" || string.IsNullOrEmpty(djfs))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("Q&" + (i + 1));
                        return;
                    }

                    ////判断点检方式是否存在
                    //strSql = "select * from CabinetRecordType where id=" + djfs ;
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("R&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    djdw = value[2];

                    if (ex.isNumber(value[3]) == true)
                    {
                        djzq = double.Parse(value[3]);
                    }
                    if (value[4] != "undefined")
                    {
                        djr = value[4];
                    }
                    else
                    {
                        djr = "";
                    }
                    if (value[5] != "undefined")
                    {
                        bjr = value[5];
                    }
                    else
                    {
                        bjr = "";
                    }


                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@djid",djfs),
                            new SqlParameter("@djdw",djdw),
                            new SqlParameter("@djzq",djzq),
                            new SqlParameter("@djr",djr),
                            new SqlParameter("@bjr",bjr)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_sbdj";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();
                }
            }
            //--------------------------------------------------------------

            //点检参数
            //--------------------------------------------------------------
            if (!string.IsNullOrEmpty(str[6].Split('*')[2]) || str[6].Split('*')[2].ToString() != "0")
            {
                SqlParameter[] pm_djcs = {
                            new SqlParameter("@sbid",id)
                        };
                SqlCommand cmd_djcs = new SqlCommand();
                cmd_djcs.Connection = con;//获取数据连接
                cmd_djcs.Transaction = sqltra;//，在执行SQL时
                cmd_djcs.CommandText = "del_sbzl_djcs";
                cmd_djcs.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in pm_djcs)
                {
                    cmd_djcs.Parameters.Add(p);
                }
                cmd_djcs.ExecuteNonQuery();
                cmd_djcs.Dispose();
                
                rCount = int.Parse(str[6].Split('*')[2]);
                string[] strDjcs = str[6].Split('*')[1].Split('@');
                string djcs = "";       //点检参数
                double xx = 0;       //下限
                double sx = 0;        //上限
                string blm = "";        //变量名
                string inumber = "";        //对应点检方式表中的inumber字段

                for (int i = 0; i < rCount; i++)
                {
                    value = strDjcs[i].Split('&');
                    djcs = value[1];

                    if (djcs.Trim() == "" || string.IsNullOrEmpty(djcs))
                    {
                        con.Close();
                        HttpContext.Current.Response.Write("S&" + (i + 1));
                        return;
                    }

                    ////判断点检参数是否存在
                    //strSql = "select * from CabinetRecordType where inumber1='" + djcs + "' or inumber2='" + djcs + "' or inumber3='" + djcs + "' or inumber4='" + djcs + "' or inumber5='" + djcs + "' or inumber6='" + djcs + "' or inumber7='" + djcs + "' or inumber8='" + djcs + "' or inumber9='" + djcs + "' or inumber10='" + djcs + "'";
                    //dtMx = SqlHelper.GetTable(strSql);
                    //if (dtMx.Rows.Count == 0)
                    //{
                    //    dtMx = null;      //出错后释放
                    //    con.Close();

                    //    HttpContext.Current.Response.Write("T&" + (i + 1));
                    //    return;
                    //}
                    //dtMx = null;

                    if (ex.isNumber(value[2]) == true)
                    {
                        xx = double.Parse(value[2]);
                    }
                    if (ex.isNumber(value[3]) == true)
                    {
                        sx = double.Parse(value[3]);
                    }

                    if (value[4] != "undefined")
                    {
                        blm = value[4];
                    }
                    else
                    {
                        blm = "";
                    }

                    //if (blm.Trim() == "" || string.IsNullOrEmpty(blm))
                    //{
                    //    con.Close();
                    //    HttpContext.Current.Response.Write("W&" + (i + 1));
                    //    return;
                    //}

                    inumber = value[5].Trim(); ;

                    SqlParameter[] parms_mx = {
                            new SqlParameter("@sbid",id),
                            new SqlParameter("@djcs",djcs),
                            new SqlParameter("@inumber",inumber),
                            new SqlParameter("@xx",xx),
                            new SqlParameter("@sx",sx),
                            new SqlParameter("@blm",blm)
                        };

                    SqlCommand cmd_mx = new SqlCommand();
                    cmd_mx.Connection = con;//获取数据连接
                    cmd_mx.Transaction = sqltra;//，在执行SQL时
                    cmd_mx.CommandText = "insert_sbzl_djcs";
                    cmd_mx.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter p in parms_mx)
                    {
                        cmd_mx.Parameters.Add(p);
                    }
                    cmd_mx.ExecuteNonQuery();
                    cmd_mx.Dispose();
                }
            }
            //--------------------------------------------------------------

            sqltra.Commit();        //提交事务

            if (con.State == ConnectionState.Open) con.Close();
            con.Dispose();

            HttpContext.Current.Response.Write("修改成功！");
            }
            catch
            {
                HttpContext.Current.Response.Write("修改失败！");
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