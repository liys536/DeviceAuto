using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DeviceAuto
{
    /// <summary>
    /// djjl 的摘要说明
    /// </summary>
    public class djjl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            string sbid=context.Request["sb"];
            string state=context.Request ["state"];
            switch (action)
            {
                case "query":
                    Query(sbid, state);
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
                case "isshow":
                    show();
                    break;
                case "export":
                    Export();
                    break;
                case "search":
                    Search();
                    break;
            }
        }

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <returns></returns>
        public void Search()
        {
            try
            {

                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

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

                string s_csszy = strr[2].Split('=')[1];
                if (s_csszy.Trim().Length > 0)
                {
                    s_where = s_where + " and izyid='" + s_csszy + "'";
                }

                string s_cscx = strr[3].Split('=')[1];
                if (s_cscx.Trim().Length > 0)
                {
                    s_where = s_where + " and iscxid = '" + s_cscx + "'";
                }

                string s_cjllx = strr[4].Split('=')[1];
                if (s_cjllx.Trim().Length > 0)
                {
                    s_where = s_where + " and typename like '%" + s_cjllx + "%'";
                }

                string s_cdjr = strr[5].Split('=')[1];
                if (s_cdjr.Trim().Length > 0)
                {
                    s_where = s_where + " and cdjr like '%" + s_cdjr + "%'";
                }


                sys ex = new sys();

                string s_dtp1 = strr[6].Split('=')[1];
                if (ex.isDate(s_dtp1) == true)
                {
                    s_dtp1 = s_dtp1.Replace("+", " ");          //替换+为空，日期提交后格式为：2016-05-30+10:00:00

                    if (s_dtp1.Trim().Length > 0)
                    {
                        s_where = s_where + " and drq>= '" + s_dtp1 + "'";
                    }
                }

                string s_dtp2 = strr[7].Split('=')[1];
                if (ex.isDate(s_dtp2) == true)
                {
                    s_dtp2 = s_dtp2.Replace("+", " ");

                    if (s_dtp2.Trim().Length > 0)
                    {
                        s_where = s_where + " and drq<= '" + s_dtp2 + "'";
                    }
                }

                DataSet duser = SqlHelper.GetList("v_djjl", "*", "id", int.Parse(rows), int.Parse(page), false, true, s_where);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_djjl where " + s_where + " order by drq desc");
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

        /// <summary>
        /// 输出excel
        /// </summary>
        private void Export()
        {
            string fn = "点检记录" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";       //使用xlsx容易出现不一致提示，打不开文件，可用wps打开
            string data = HttpContext.Current.Request["data"];
            File.WriteAllText(HttpContext.Current.Server.MapPath(fn), data, Encoding.UTF8);//如果是gb2312的xml申明，第三个编码参数修改为Encoding.GetEncoding(936)

            HttpContext.Current.Response.Write(fn);//返回文件名提供下载
        }

        //列是否显示
        private void show()
        {
            try
            {
                StringBuilder sb = new StringBuilder("1=1");
                string ssbid = HttpContext.Current.Request["data"];

                if (string.IsNullOrEmpty(ssbid)) return;

                string value = "";
                DataTable dt = new DataTable();
                dt = SqlHelper.GetTable("select c.* from CabinetRecordType c inner join sbzlb_djfs sd on c.id=sd.idjid " +
                                    " inner join sbzlb s on sd.isbid=s.id where s.id='" + ssbid + "'");

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["cStr1"] == null || dt.Rows[0]["cStr1"] == "")
                    {
                        value = "cStr1@f" + "*";
                    }
                    else
                    {
                        value = "cStr1@t@" + dt.Rows[0]["cStr1"] + "*";
                    }
                    if (dt.Rows[0]["cStr2"] == null || dt.Rows[0]["cStr2"] == "")
                    {
                        value = value + "cStr2@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr2@t@" + dt.Rows[0]["cStr2"] + "*";
                    }
                    if (dt.Rows[0]["cStr3"] == null || dt.Rows[0]["cStr3"] == "")
                    {
                        value = value + "cStr3@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr3@t@" + dt.Rows[0]["cStr3"] + "*";
                    }
                    if (dt.Rows[0]["cStr4"] == null || dt.Rows[0]["cStr4"] == "")
                    {
                        value = value + "cStr4@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr4@t@" + dt.Rows[0]["cStr4"] + "*";
                    }
                    if (dt.Rows[0]["cStr5"] == null || dt.Rows[0]["cStr5"] == "")
                    {
                        value = value + "cStr5@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr5@t@" + dt.Rows[0]["cStr5"] + "*";
                    }
                    if (dt.Rows[0]["cStr6"] == null || dt.Rows[0]["cStr6"] == "")
                    {
                        value = value + "cStr6@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr6@t@" + dt.Rows[0]["cStr6"] + "*";
                    }
                    if (dt.Rows[0]["cStr7"] == null || dt.Rows[0]["cStr7"] == "")
                    {
                        value = value + "cStr7@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr7@t@" + dt.Rows[0]["cStr7"] + "*";
                    }
                    if (dt.Rows[0]["cStr8"] == null || dt.Rows[0]["cStr8"] == "")
                    {
                        value = value + "cStr8@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr8@t@" + dt.Rows[0]["cStr8"] + "*";
                    }
                    if (dt.Rows[0]["cStr9"] == null || dt.Rows[0]["cStr9"] == "")
                    {
                        value = value + "cStr9@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr9@t@" + dt.Rows[0]["cStr9"] + "*";
                    }
                    if (dt.Rows[0]["cStr10"] == null || dt.Rows[0]["cStr10"] == "")
                    {
                        value = value + "cStr10@f" + "*";
                    }
                    else
                    {
                        value = value + "cStr10@t@" + dt.Rows[0]["cStr10"] + "*";
                    }

                    if (dt.Rows[0]["bBool1"] == null || dt.Rows[0]["bBool1"] == "")
                    {
                        value = value + "bBool1@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool1@t@" + dt.Rows[0]["bBool1"] + "*";
                    }
                    if (dt.Rows[0]["bBool2"] == null || dt.Rows[0]["bBool2"] == "")
                    {
                        value = value + "bBool2@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool2@t@" + dt.Rows[0]["bBool2"] + "*";
                    }
                    if (dt.Rows[0]["bBool3"] == null || dt.Rows[0]["bBool3"] == "")
                    {
                        value = value + "bBool3@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool3@t@" + dt.Rows[0]["bBool3"] + "*";
                    }
                    if (dt.Rows[0]["bBool4"] == null || dt.Rows[0]["bBool4"] == "")
                    {
                        value = value + "bBool4@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool4@t@" + dt.Rows[0]["bBool4"] + "*";
                    }
                    if (dt.Rows[0]["bBool5"] == null || dt.Rows[0]["bBool5"] == "")
                    {
                        value = value + "bBool5@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool5@t@" + dt.Rows[0]["bBool5"] + "*";
                    }
                    if (dt.Rows[0]["bBool6"] == null || dt.Rows[0]["bBool6"] == "")
                    {
                        value = value + "bBool6@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool6@t@" + dt.Rows[0]["bBool6"] + "*";
                    }
                    if (dt.Rows[0]["bBool7"] == null || dt.Rows[0]["bBool7"] == "")
                    {
                        value = value + "bBool7@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool7@t@" + dt.Rows[0]["bBool7"] + "*";
                    }
                    if (dt.Rows[0]["bBool8"] == null || dt.Rows[0]["bBool8"] == "")
                    {
                        value = value + "bBool8@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool8@t@" + dt.Rows[0]["bBool8"] + "*";
                    }
                    if (dt.Rows[0]["bBool9"] == null || dt.Rows[0]["bBool9"] == "")
                    {
                        value = value + "bBool9@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool9@t@" + dt.Rows[0]["bBool9"] + "*";
                    }
                    if (dt.Rows[0]["bBool10"] == null || dt.Rows[0]["bBool10"] == "")
                    {
                        value = value + "bBool10@f" + "*";
                    }
                    else
                    {
                        value = value + "bBool10@t@" + dt.Rows[0]["bBool10"] + "*";
                    }

                    if (dt.Rows[0]["iNumber1"] == null || dt.Rows[0]["iNumber1"] == "")
                    {
                        value = value + "iNumber1@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber1@t@" + dt.Rows[0]["iNumber1"] + "*";
                    }
                    if (dt.Rows[0]["iNumber2"] == null || dt.Rows[0]["iNumber2"] == "")
                    {
                        value = value + "iNumber2@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber2@t@" + dt.Rows[0]["iNumber2"] + "*";
                    }
                    if (dt.Rows[0]["iNumber3"] == null || dt.Rows[0]["iNumber3"] == "")
                    {
                        value = value + "iNumber3@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber3@t@" + dt.Rows[0]["iNumber3"] + "*";
                    }
                    if (dt.Rows[0]["iNumber4"] == null || dt.Rows[0]["iNumber4"] == "")
                    {
                        value = value + "iNumber4@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber4@t@" + dt.Rows[0]["iNumber4"] + "*";
                    }
                    if (dt.Rows[0]["iNumber5"] == null || dt.Rows[0]["iNumber5"] == "")
                    {
                        value = value + "iNumber@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber5@t@" + dt.Rows[0]["iNumber5"] + "*";
                    }
                    if (dt.Rows[0]["iNumber6"] == null || dt.Rows[0]["iNumber6"] == "")
                    {
                        value = value + "iNumber6@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber6@t@" + dt.Rows[0]["iNumber6"] + "*";
                    }
                    if (dt.Rows[0]["iNumber7"] == null || dt.Rows[0]["iNumber7"] == "")
                    {
                        value = value + "iNumber7@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber7@t@" + dt.Rows[0]["iNumber7"] + "*";
                    }
                    if (dt.Rows[0]["iNumber8"] == null || dt.Rows[0]["iNumber8"] == "")
                    {
                        value = value + "iNumber8@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber8@t@" + dt.Rows[0]["iNumber8"] + "*";
                    }
                    if (dt.Rows[0]["iNumber9"] == null || dt.Rows[0]["iNumber9"] == "")
                    {
                        value = value + "iNumber9@f" + "*";
                    }
                    else
                    {
                        value = value + "iNumber9@t@" + dt.Rows[0]["iNumber9"] + "*";
                    }
                    if (dt.Rows[0]["iNumber10"] == null || dt.Rows[0]["iNumber10"] == "")
                    {
                        value = value + "iNumber10@f";
                    }
                    else
                    {
                        value = value + "iNumber10@t@" + dt.Rows[0]["iNumber10"];
                    }

                }

                HttpContext.Current.Response.Write(value);
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
            }
        }

        /// <summary>
        /// 查询的方法
        /// </summary>
        private void Query(string isbid,string sState)
        {
            try
            {
                if (sState == "" || string.IsNullOrEmpty(sState))
                {
                    HttpContext.Current.Response.Write("{ \"total\":0,\"rows\":[]");
                    return;
                }

                //一页显示几行数据
                string rows = HttpContext.Current.Request["rows"];
                //当前页
                string page = HttpContext.Current.Request["page"];

                string strWhere = "1=1";

                if (!string.IsNullOrEmpty(sState) && sState =="s")
                {
                    if (!string.IsNullOrEmpty(isbid))
                    {
                        strWhere = strWhere + " and iSbid=" + isbid;
                    }
                }

                string qsrq = HttpContext.Current.Request["qsrq"];
                if (!string.IsNullOrEmpty(qsrq))
                {
                    strWhere = strWhere + " and drq>='" + qsrq + "'";
                }

                string jzrq = HttpContext.Current.Request["jzrq"];
                if (!string.IsNullOrEmpty(jzrq))
                {
                    strWhere = strWhere + " and drq<='" + jzrq + "'";
                }

                string ckey = HttpContext.Current.Request["ckey"];
                string ctext = HttpContext.Current.Request["cmemo"];

                if (!string.IsNullOrEmpty(ckey))
                {
                    switch (ckey)
                    {
                        case "设备编号":
                            strWhere = strWhere + " and csbbh like '%" + ctext + "%'";
                            break;
                        case "设备名称":
                            strWhere = strWhere + " and csbmc like '%" + ctext + "%'";
                            break;
                        case "生产线":
                            strWhere = strWhere + " and cscx like '%" + ctext + "%'";
                            break;
                        case "所属专业":
                            strWhere = strWhere + " and csszy like '%" + ctext + "%'";
                            break;
                        case "点检人":
                            strWhere = strWhere + " and cdjr like '%" + ctext + "%'";
                            break;
                        case "备注":
                            strWhere = strWhere + " and cbz like '%" + ctext + "%'";
                            break;
                    }

                }

                DataSet duser = SqlHelper.GetList("v_djjl", "*", "id", int.Parse(rows), int.Parse(page), false, true, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_djjl where " + strWhere + " order by drq desc");
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

        //删除的方法
        private void Del()
        {
            try
            {
                //获取到选中行的id
                string id = HttpContext.Current.Request["id"];
                int count = 0;
                count = SqlHelper.DelData("djjlb", id);
                if (count > 0)
                {
                    HttpContext.Current.Response.Write("共删除了" + count + "条数据");
                }
                else
                {
                    HttpContext.Current.Response.Write("error");
                }
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
                string[] str = ss.Split('@');
                string csbbh = str[2];

                if (csbbh.Trim() == "")
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from sbzlb where DeviceId='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                if (str[3] == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }
                sys ex = new sys();
                string drq = "";
                if (ex.isDate(str[3]) == true)
                {
                    drq = str[3];
                }
                string cdjr = str[4];
                string cbz = str[5];
                string cstr1 = str[6];
                string cstr2 = str[7];
                string cstr3 = str[8];
                string cstr4 = str[9];
                string cstr5 = str[10];
                string cstr6 = str[11];
                string cstr7 = str[12];
                string cstr8 = str[13];
                string cstr9 = str[14];
                string cstr10 = str[15];
                string bbool1 = str[16];
                string bbool2 = str[17];
                string bbool3 = str[18];
                string bbool4 = str[19];
                string bbool5 = str[20];
                string bbool6 = str[21];
                string bbool7 = str[22];
                string bbool8 = str[23];
                string bbool9 = str[24];
                string bbool10 = str[25];
                string inumber1 = str[26];
                string inumber2 = str[27];
                string inumber3 = str[28];
                string inumber4 = str[29];
                string inumber5 = str[30];
                string inumber6 = str[31];
                string inumber7 = str[32];
                string inumber8 = str[33];
                string inumber9 = str[34];
                string inumber10 = str[35];

                SqlParameter[] parms = {
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@djr",cdjr),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@cstr1",cstr1),
                            new SqlParameter("@cstr2",cstr2),
                            new SqlParameter("@cstr3",cstr3),
                            new SqlParameter("@cstr4",cstr4),
                            new SqlParameter("@cstr5",cstr5),
                            new SqlParameter("@cstr6",cstr6),
                            new SqlParameter("@cstr7",cstr7),
                            new SqlParameter("@cstr8",cstr8),
                            new SqlParameter("@cstr9",cstr9),
                            new SqlParameter("@cstr10",cstr10),
                            new SqlParameter("@bBool1",bbool1),
                            new SqlParameter("@bBool2",bbool2),
                            new SqlParameter("@bBool3",bbool3),
                            new SqlParameter("@bBool4",bbool4),
                            new SqlParameter("@bBool5",bbool5),
                            new SqlParameter("@bBool6",bbool6),
                            new SqlParameter("@bBool7",bbool7),
                            new SqlParameter("@bBool8",bbool8),
                            new SqlParameter("@bBool9",bbool9),
                            new SqlParameter("@bBool10",bbool10),
                            new SqlParameter("@iNumber1",inumber1),
                            new SqlParameter("@iNumber2",inumber2),
                            new SqlParameter("@iNumber3",inumber3),
                            new SqlParameter("@iNumber4",inumber4),
                            new SqlParameter("@iNumber5",inumber5),
                            new SqlParameter("@iNumber6",inumber6),
                            new SqlParameter("@iNumber7",inumber7),
                            new SqlParameter("@iNumber8",inumber8),
                            new SqlParameter("@iNumber9",inumber9),
                            new SqlParameter("@iNumber10",inumber10)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_djjl", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("添加成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("添加失败！");
                }
            }
            catch (Exception ex)
            {
                sys e = new sys();
                e.GetLog(ex);
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
                string[] str = ss.Split('@');
                int id = int.Parse(str[1]);
                string csbbh = str[2];

                if (csbbh.Trim() == "")
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                DataTable dt = SqlHelper.GetTable("select * from sbzlb where DeviceId='" + csbbh + "'");
                if (dt.Rows.Count == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                if (str[3] == "")
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }
                sys ex = new sys();
                string drq = "";
                if (ex.isDate(str[3]) == true)
                {
                    drq = str[3];
                }
                string cdjr = str[4];
                string cbz = str[5];
                string cstr1 = str[6];
                string cstr2 = str[7];
                string cstr3 = str[8];
                string cstr4 = str[9];
                string cstr5 = str[10];
                string cstr6 = str[11];
                string cstr7 = str[12];
                string cstr8 = str[13];
                string cstr9 = str[14];
                string cstr10 = str[15];
                string bbool1 = str[16];
                string bbool2 = str[17];
                string bbool3 = str[18];
                string bbool4 = str[19];
                string bbool5 = str[20];
                string bbool6 = str[21];
                string bbool7 = str[22];
                string bbool8 = str[23];
                string bbool9 = str[24];
                string bbool10 = str[25];
                string inumber1 = str[26];
                string inumber2 = str[27];
                string inumber3 = str[28];
                string inumber4 = str[29];
                string inumber5 = str[30];
                string inumber6 = str[31];
                string inumber7 = str[32];
                string inumber8 = str[33];
                string inumber9 = str[34];
                string inumber10 = str[35];

                SqlParameter[] parms = {
                            new SqlParameter("@sbbh",csbbh),
                            new SqlParameter("@rq",drq),
                            new SqlParameter("@djr",cdjr),
                            new SqlParameter("@bz",cbz),
                            new SqlParameter("@cstr1",cstr1),
                            new SqlParameter("@cstr2",cstr2),
                            new SqlParameter("@cstr3",cstr3),
                            new SqlParameter("@cstr4",cstr4),
                            new SqlParameter("@cstr5",cstr5),
                            new SqlParameter("@cstr6",cstr6),
                            new SqlParameter("@cstr7",cstr7),
                            new SqlParameter("@cstr8",cstr8),
                            new SqlParameter("@cstr9",cstr9),
                            new SqlParameter("@cstr10",cstr10),
                            new SqlParameter("@bBool1",bbool1),
                            new SqlParameter("@bBool2",bbool2),
                            new SqlParameter("@bBool3",bbool3),
                            new SqlParameter("@bBool4",bbool4),
                            new SqlParameter("@bBool5",bbool5),
                            new SqlParameter("@bBool6",bbool6),
                            new SqlParameter("@bBool7",bbool7),
                            new SqlParameter("@bBool8",bbool8),
                            new SqlParameter("@bBool9",bbool9),
                            new SqlParameter("@bBool10",bbool10),
                            new SqlParameter("@iNumber1",inumber1),
                            new SqlParameter("@iNumber2",inumber2),
                            new SqlParameter("@iNumber3",inumber3),
                            new SqlParameter("@iNumber4",inumber4),
                            new SqlParameter("@iNumber5",inumber5),
                            new SqlParameter("@iNumber6",inumber6),
                            new SqlParameter("@iNumber7",inumber7),
                            new SqlParameter("@iNumber8",inumber8),
                            new SqlParameter("@iNumber9",inumber9),
                            new SqlParameter("@iNumber10",inumber10),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_djjl", CommandType.StoredProcedure, parms);
                if (flag)
                {
                    HttpContext.Current.Response.Write("修改成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("修改失败！");
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