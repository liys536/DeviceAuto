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
    /// djfs 的摘要说明
    /// </summary>
    public class djfs : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"];
            switch (action)
            {
                case "query":
                    Query();
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
            }
        }

        /// <summary>
        /// 组合搜索条件
        /// </summary>
        /// <returns></returns>
        private string GetWhere()
        {
            StringBuilder sb = new StringBuilder("1=1");
            string searchType = HttpContext.Current.Request["search_type"] != "" ? HttpContext.Current.Request["search_type"] : string.Empty;
            string searchValue = HttpContext.Current.Request["search_value"] != "" ? HttpContext.Current.Request["search_value"] : string.Empty;
            //string searchType = "";
            //string searchValue = "";
            if (searchType != null && searchValue != null)
            {
                //sb.AppendFormat(" and charindex('{0}',{1})>0", searchValue, searchType);
                sb.AppendFormat(" and {0} like '%{1}%'", searchType, searchValue);
            }
            return sb.ToString();
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

                DataSet duser = SqlHelper.GetList("v_CabinetRecordType", "*", "id", int.Parse(rows), int.Parse(page), false, false, strWhere);
                DataTable dt1 = duser.Tables[0];
                //获取数据源
                DataTable dt = SqlHelper.GetTable("select * from v_CabinetRecordType where " + strWhere);
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

                string sSql = "";
                DataTable dt = new DataTable();

                //判断停机记录中是否存在
                sSql = "select * from sbzlb_djfs where idjid=" + id;
                dt = SqlHelper.GetTable(sSql);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }

                int count = 0;
                count = SqlHelper.DelData("CabinetRecordType", id);
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
                    sb.AppendFormat("{0}:{1}", s, HttpContext.Current.Request.Form[s]);
                }
                string ss = sb.ToString();
                string[] str = ss.Split('&');
                string TypeName = str[1].Split('=')[1];

                if (TypeName.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                //判断记录类型是否存在
                DataTable dt = SqlHelper.GetTable("select * from CabinetRecordType where TypeName='" + TypeName + "'");
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string jlzy = str[2].Split('=')[1];

                string TypeDesigner = str[3].Split('=')[1];

                if (TypeDesigner.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }
                string DesignDate = "";
                sys ex = new sys();
                if (ex.isDate(str[4].Split('=')[1]) == true)
                {
                    DesignDate = str[4].Split('=')[1];
                }
                string ColumnInformation = str[5].Split('=')[1];
                if (ColumnInformation.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("4");
                    return;
                }


                string[] strChar = ColumnInformation.Split('#')[0].Split('*');
                int cnt = int.Parse(ColumnInformation.Split('#')[1]);
                string value = "<RecordColInfo Count=\"" + cnt + "\">";

                string cstr1 = strChar[0].Split('@')[1];
                if (cstr1 != "")
                {
                    //输入双引号必须用在前面加字符\
                    value = value + "<cStr1 Type=\"System.String\">" + cstr1 + "</cStr1>";
                }
                string cstr2 = strChar[1].Split('@')[1];
                if (cstr2 != "")
                {
                    value = value + "<cStr2 Type=\"System.String\">" + cstr2 + "</cStr2>";
                }
                string cstr3 = strChar[2].Split('@')[1];
                if (cstr3 != "")
                {
                    value = value + "<cStr3 Type=\"System.String\">" + cstr3 + "</cStr3>";
                }
                string cstr4 = strChar[3].Split('@')[1];
                if (cstr4 != "")
                {
                    value = value + "<cStr4 Type=\"System.String\">" + cstr4 + "</cStr4>";
                }
                string cstr5 = strChar[4].Split('@')[1];
                if (cstr5 != "")
                {
                    value = value + "<cStr5 Type=\"System.String\">" + cstr5 + "</cStr5>";
                }
                string cstr6 = strChar[5].Split('@')[1];
                if (cstr6 != "")
                {
                    value = value + "<cStr6 Type=\"System.String\">" + cstr6 + "</cStr6>";
                }
                string cstr7 = strChar[6].Split('@')[1];
                if (cstr7 != "")
                {
                    value = value + "<cStr7 Type=\"System.String\">" + cstr7 + "</cStr7>";
                }
                string cstr8 = strChar[7].Split('@')[1];
                if (cstr8 != "")
                {
                    value = value + "<cStr8 Type=\"System.String\">" + cstr8 + "</cStr8>";
                }
                string cstr9 = strChar[8].Split('@')[1];
                if (cstr9 != "")
                {
                    value = value + "<cStr9 Type=\"System.String\">" + cstr9 + "</cStr9>";
                }
                string cstr10 = strChar[9].Split('@')[1];
                if (cstr10 != "")
                {
                    value = value + "<cStr10 Type=\"System.String\">" + cstr10 + "</cStr10>";
                }
                string bBool1 = strChar[10].Split('@')[1];
                if (bBool1 != "")
                {
                    value = value + "<bBool1 Type=\"System.Boolean\">" + bBool1 + "</bBool1>";
                }
                string bBool2 = strChar[11].Split('@')[1];
                if (bBool2 != "")
                {
                    value = value + "<bBool2 Type=\"System.Boolean\">" + bBool2 + "</bBool2>";
                }
                string bBool3 = strChar[12].Split('@')[1];
                if (bBool3 != "")
                {
                    value = value + "<bBool3 Type=\"System.Boolean\">" + bBool3 + "</bBool3>";
                }
                string bBool4 = strChar[13].Split('@')[1];
                if (bBool4 != "")
                {
                    value = value + "<bBool4 Type=\"System.Boolean\">" + bBool4 + "</bBool4>";
                }
                string bBool5 = strChar[14].Split('@')[1];
                if (bBool5 != "")
                {
                    value = value + "<bBool5 Type=\"System.Boolean\">" + bBool5 + "</bBool5>";
                }
                string bBool6 = strChar[15].Split('@')[1];
                if (bBool6 != "")
                {
                    value = value + "<bBool6 Type=\"System.Boolean\">" + bBool6 + "</bBool6>";
                }
                string bBool7 = strChar[16].Split('@')[1];
                if (bBool7 != "")
                {
                    value = value + "<bBool7 Type=\"System.Boolean\">" + bBool7 + "</bBool7>";
                }
                string bBool8 = strChar[17].Split('@')[1];
                if (bBool8 != "")
                {
                    value = value + "<bBool8 Type=\"System.Boolean\">" + bBool8 + "</bBool8>";
                }
                string bBool9 = strChar[18].Split('@')[1];
                if (bBool9 != "")
                {
                    value = value + "<bBool9 Type=\"System.Boolean\">" + bBool9 + "</bBool9>";
                }
                string bBool10 = strChar[19].Split('@')[1];
                if (bBool10 != "")
                {
                    value = value + "<bBool10 Type=\"System.Boolean\">" + bBool10 + "</bBool10>";
                }
                string iNumber1 = strChar[20].Split('@')[1];
                if (iNumber1 != "")
                {
                    value = value + "<iNumber1 Type=\"System.Decimal\">" + iNumber1 + "</iNumber1>";
                }
                string iNumber2 = strChar[21].Split('@')[1];
                if (iNumber2 != "")
                {
                    value = value + "<iNumber2 Type=\"System.Decimal\">" + iNumber2 + "</iNumber2>";
                }
                string iNumber3 = strChar[22].Split('@')[1];
                if (iNumber3 != "")
                {
                    value = value + "<iNumber3 Type=\"System.Decimal\">" + iNumber3 + "</iNumber3>";
                }
                string iNumber4 = strChar[23].Split('@')[1];
                if (iNumber4 != "")
                {
                    value = value + "<iNumber4 Type=\"System.Decimal\">" + iNumber4 + "</iNumber4>";
                }
                string iNumber5 = strChar[24].Split('@')[1];
                if (iNumber5 != "")
                {
                    value = value + "<iNumber5 Type=\"System.Decimal\">" + iNumber5 + "</iNumber5>";
                }
                string iNumber6 = strChar[25].Split('@')[1];
                if (iNumber6 != "")
                {
                    value = value + "<iNumber6 Type=\"System.Decimal\">" + iNumber6 + "</iNumber6>";
                }
                string iNumber7 = strChar[26].Split('@')[1];
                if (iNumber7 != "")
                {
                    value = value + "<iNumber7 Type=\"System.Decimal\">" + iNumber7 + "</iNumber7>";
                }
                string iNumber8 = strChar[27].Split('@')[1];
                if (iNumber8 != "")
                {
                    value = value + "<iNumber8 Type=\"System.Decimal\">" + iNumber8 + "</iNumber8>";
                }
                string iNumber9 = strChar[28].Split('@')[1];
                if (iNumber9 != "")
                {
                    value = value + "<iNumber9 Type=\"System.Decimal\">" + iNumber9 + "</iNumber9>";
                }
                string iNumber10 = strChar[29].Split('@')[1];
                if (iNumber10 != "")
                {
                    value = value + "<iNumber10 Type=\"System.Decimal\">" + iNumber10 + "</iNumber10>";
                }

                value = value + "</RecordColInfo>";

                SqlParameter[] parms = {
                            new SqlParameter("@TypeName",TypeName),
                            new SqlParameter("@jlzy",jlzy),
                            new SqlParameter("@TypeDesigner",TypeDesigner),
                            new SqlParameter("@DesignDate",DesignDate),
                            new SqlParameter("@ColumnInformation",value),
                            new SqlParameter("@ColumnInformation2",ColumnInformation),
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
                            new SqlParameter("@bBool1",bBool1),
                            new SqlParameter("@bBool2",bBool2),
                            new SqlParameter("@bBool3",bBool3),
                            new SqlParameter("@bBool4",bBool4),
                            new SqlParameter("@bBool5",bBool5),
                            new SqlParameter("@bBool6",bBool6),
                            new SqlParameter("@bBool7",bBool7),
                            new SqlParameter("@bBool8",bBool8),
                            new SqlParameter("@bBool9",bBool9),
                            new SqlParameter("@bBool10",bBool10),
                            new SqlParameter("@iNumber1",iNumber1),
                            new SqlParameter("@iNumber2",iNumber2),
                            new SqlParameter("@iNumber3",iNumber3),
                            new SqlParameter("@iNumber4",iNumber4),
                            new SqlParameter("@iNumber5",iNumber5),
                            new SqlParameter("@iNumber6",iNumber6),
                            new SqlParameter("@iNumber7",iNumber7),
                            new SqlParameter("@iNumber8",iNumber8),
                            new SqlParameter("@iNumber9",iNumber9),
                            new SqlParameter("@iNumber10",iNumber10)
                                 };
                bool flag = SqlHelper.ExeNonQuery("insert_CabinetRecordType", CommandType.StoredProcedure, parms);
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
                string[] str = ss.Split('&');
                int id = int.Parse(str[0].Split('=')[1]);
                string TypeName = str[1].Split('=')[1];
                if (TypeName.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("1");
                    return;
                }
                //判断记录类型是否存在
                DataTable dt = SqlHelper.GetTable("select * from CabinetRecordType where TypeName='" + TypeName + "' and id<>" + id);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Response.Write("3");
                    return;
                }

                string jlzy = str[2].Split('=')[1];

                string TypeDesigner = str[3].Split('=')[1];
                if (TypeDesigner.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("2");
                    return;
                }

                string DesignDate = "";
                sys ex = new sys();
                if (ex.isDate(str[4].Split('=')[1]) == true)
                {
                    DesignDate = str[4].Split('=')[1];
                }
                string ColumnInformation = str[5].Split('=')[1];
                if (ColumnInformation.Trim().Length == 0)
                {
                    HttpContext.Current.Response.Write("4");
                    return;
                }

                string[] strChar = ColumnInformation.Split('#')[0].Split('*');
                int cnt = int.Parse(ColumnInformation.Split('#')[1]);
                string value = "<RecordColInfo Count=\"" + cnt + "\">";

                string cstr1 = strChar[0].Split('@')[1];
                if (cstr1 != "")
                {
                    //输入双引号必须用在前面加字符\
                    value = value + "<cStr1 Type=\"System.String\">" + cstr1 + "</cStr1>";
                }
                string cstr2 = strChar[1].Split('@')[1];
                if (cstr2 != "")
                {
                    value = value + "<cStr2 Type=\"System.String\">" + cstr2 + "</cStr2>";
                }
                string cstr3 = strChar[2].Split('@')[1];
                if (cstr3 != "")
                {
                    value = value + "<cStr3 Type=\"System.String\">" + cstr3 + "</cStr3>";
                }
                string cstr4 = strChar[3].Split('@')[1];
                if (cstr4 != "")
                {
                    value = value + "<cStr4 Type=\"System.String\">" + cstr4 + "</cStr4>";
                }
                string cstr5 = strChar[4].Split('@')[1];
                if (cstr5 != "")
                {
                    value = value + "<cStr5 Type=\"System.String\">" + cstr5 + "</cStr5>";
                }
                string cstr6 = strChar[5].Split('@')[1];
                if (cstr6 != "")
                {
                    value = value + "<cStr6 Type=\"System.String\">" + cstr6 + "</cStr6>";
                }
                string cstr7 = strChar[6].Split('@')[1];
                if (cstr7 != "")
                {
                    value = value + "<cStr7 Type=\"System.String\">" + cstr7 + "</cStr7>";
                }
                string cstr8 = strChar[7].Split('@')[1];
                if (cstr8 != "")
                {
                    value = value + "<cStr8 Type=\"System.String\">" + cstr8 + "</cStr8>";
                }
                string cstr9 = strChar[8].Split('@')[1];
                if (cstr9 != "")
                {
                    value = value + "<cStr9 Type=\"System.String\">" + cstr9 + "</cStr9>";
                }
                string cstr10 = strChar[9].Split('@')[1];
                if (cstr10 != "")
                {
                    value = value + "<cStr10 Type=\"System.String\">" + cstr10 + "</cStr10>";
                }
                string bBool1 = strChar[10].Split('@')[1];
                if (bBool1 != "")
                {
                    value = value + "<bBool1 Type=\"System.Boolean\">" + bBool1 + "</bBool1>";
                }
                string bBool2 = strChar[11].Split('@')[1];
                if (bBool2 != "")
                {
                    value = value + "<bBool2 Type=\"System.Boolean\">" + bBool2 + "</bBool2>";
                }
                string bBool3 = strChar[12].Split('@')[1];
                if (bBool3 != "")
                {
                    value = value + "<bBool3 Type=\"System.Boolean\">" + bBool3 + "</bBool3>";
                }
                string bBool4 = strChar[13].Split('@')[1];
                if (bBool4 != "")
                {
                    value = value + "<bBool4 Type=\"System.Boolean\">" + bBool4 + "</bBool4>";
                }
                string bBool5 = strChar[14].Split('@')[1];
                if (bBool5 != "")
                {
                    value = value + "<bBool5 Type=\"System.Boolean\">" + bBool5 + "</bBool5>";
                }
                string bBool6 = strChar[15].Split('@')[1];
                if (bBool6 != "")
                {
                    value = value + "<bBool6 Type=\"System.Boolean\">" + bBool6 + "</bBool6>";
                }
                string bBool7 = strChar[16].Split('@')[1];
                if (bBool7 != "")
                {
                    value = value + "<bBool7 Type=\"System.Boolean\">" + bBool7 + "</bBool7>";
                }
                string bBool8 = strChar[17].Split('@')[1];
                if (bBool8 != "")
                {
                    value = value + "<bBool8 Type=\"System.Boolean\">" + bBool8 + "</bBool8>";
                }
                string bBool9 = strChar[18].Split('@')[1];
                if (bBool9 != "")
                {
                    value = value + "<bBool9 Type=\"System.Boolean\">" + bBool9 + "</bBool9>";
                }
                string bBool10 = strChar[19].Split('@')[1];
                if (bBool10 != "")
                {
                    value = value + "<bBool10 Type=\"System.Boolean\">" + bBool10 + "</bBool10>";
                }
                string iNumber1 = strChar[20].Split('@')[1];
                if (iNumber1 != "")
                {
                    value = value + "<iNumber1 Type=\"System.Decimal\">" + iNumber1 + "</iNumber1>";
                }
                string iNumber2 = strChar[21].Split('@')[1];
                if (iNumber2 != "")
                {
                    value = value + "<iNumber2 Type=\"System.Decimal\">" + iNumber2 + "</iNumber2>";
                }
                string iNumber3 = strChar[22].Split('@')[1];
                if (iNumber3 != "")
                {
                    value = value + "<iNumber3 Type=\"System.Decimal\">" + iNumber3 + "</iNumber3>";
                }
                string iNumber4 = strChar[23].Split('@')[1];
                if (iNumber4 != "")
                {
                    value = value + "<iNumber4 Type=\"System.Decimal\">" + iNumber4 + "</iNumber4>";
                }
                string iNumber5 = strChar[24].Split('@')[1];
                if (iNumber5 != "")
                {
                    value = value + "<iNumber5 Type=\"System.Decimal\">" + iNumber5 + "</iNumber5>";
                }
                string iNumber6 = strChar[25].Split('@')[1];
                if (iNumber6 != "")
                {
                    value = value + "<iNumber6 Type=\"System.Decimal\">" + iNumber6 + "</iNumber6>";
                }
                string iNumber7 = strChar[26].Split('@')[1];
                if (iNumber7 != "")
                {
                    value = value + "<iNumber7 Type=\"System.Decimal\">" + iNumber7 + "</iNumber7>";
                }
                string iNumber8 = strChar[27].Split('@')[1];
                if (iNumber8 != "")
                {
                    value = value + "<iNumber8 Type=\"System.Decimal\">" + iNumber8 + "</iNumber8>";
                }
                string iNumber9 = strChar[28].Split('@')[1];
                if (iNumber9 != "")
                {
                    value = value + "<iNumber9 Type=\"System.Decimal\">" + iNumber9 + "</iNumber9>";
                }
                string iNumber10 = strChar[29].Split('@')[1];
                if (iNumber10 != "")
                {
                    value = value + "<iNumber10 Type=\"System.Decimal\">" + iNumber10 + "</iNumber10>";
                }

                value = value + "</RecordColInfo>";

                SqlParameter[] parms = {
                            new SqlParameter("@TypeName",TypeName),
                            new SqlParameter("@jlzy",jlzy),
                            new SqlParameter("@TypeDesigner",TypeDesigner),
                            new SqlParameter("@DesignDate",DesignDate),
                            new SqlParameter("@ColumnInformation",value),
                            new SqlParameter("@ColumnInformation2",ColumnInformation),
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
                            new SqlParameter("@bBool1",bBool1),
                            new SqlParameter("@bBool2",bBool2),
                            new SqlParameter("@bBool3",bBool3),
                            new SqlParameter("@bBool4",bBool4),
                            new SqlParameter("@bBool5",bBool5),
                            new SqlParameter("@bBool6",bBool6),
                            new SqlParameter("@bBool7",bBool7),
                            new SqlParameter("@bBool8",bBool8),
                            new SqlParameter("@bBool9",bBool9),
                            new SqlParameter("@bBool10",bBool10),
                            new SqlParameter("@iNumber1",iNumber1),
                            new SqlParameter("@iNumber2",iNumber2),
                            new SqlParameter("@iNumber3",iNumber3),
                            new SqlParameter("@iNumber4",iNumber4),
                            new SqlParameter("@iNumber5",iNumber5),
                            new SqlParameter("@iNumber6",iNumber6),
                            new SqlParameter("@iNumber7",iNumber7),
                            new SqlParameter("@iNumber8",iNumber8),
                            new SqlParameter("@iNumber9",iNumber9),
                            new SqlParameter("@iNumber10",iNumber10),
                            new SqlParameter("@id",id)
                                 };
                bool flag = SqlHelper.ExeNonQuery("update_CabinetRecordType", CommandType.StoredProcedure, parms);
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