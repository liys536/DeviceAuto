using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

    public class JSonHelper
    {

        public static string CreateJson(DataTable table)
        {
            string jsname = "total";
            StringBuilder json = new StringBuilder("{\""+jsname+"\":[");
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    json.Append("{");
                    foreach (DataColumn column in table.Columns)
                    {
                        json.Append("\""+column.ColumnName+"\":\""+row[column.ColumnName].ToString()+"\",");
                    }
                    json.Remove(json.Length - 1, 1);
                    json.Append("},");
                }
                json.Remove(json.Length - 1, 1);
            }
            json.Append("]}");
            return json.ToString();
        }

        public static string CreateJsonParameters(DataTable dt, bool displayCount, int totalcount)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        
            if (dt != null)
            {
                JsonString.Append("{ ");
                if (displayCount)
                {
                    JsonString.Append("\"total\":");
                    JsonString.Append(totalcount);
                    JsonString.Append(",");
                }
                JsonString.Append("\"rows\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" +
                                                  dt.Rows[i][j].ToString().ToLower() + ",");
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\",");
                            }
                            else
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" + "\"" + dt.Rows[i][j] + "\",");
                            }
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" +
                                                  dt.Rows[i][j].ToString().ToLower());
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"");
                            }
                            else
                            {
                                JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName.ToLower() + "\":" + "\"" + dt.Rows[i][j] + "\"");
                            }
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]");
                JsonString.Append("}");
                return JsonString.ToString().Replace("\n", "");
            }
            else
            {
                return null;
            }
        }
        public static string DataTableToJson(DataTable table, string name)
        {
            StringBuilder Json = new StringBuilder("{\""+name+"\":[");
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Json.Append("{");
                    foreach (DataColumn cloumn in table.Columns)
                    {
                        Json.Append("\""+cloumn.ColumnName+"\":\""+row[cloumn.ColumnName].ToString()+"\",");
                    }
                    Json.Remove(Json.Length - 1, 1);
                    Json.Append("},");
                }
                Json.Remove(Json.Length - 1, 1);
            }
            Json.Append("]}");
            return Json.ToString();
        }


            /// <summary>
            /// 将对象序列化为JSON格式
            /// </summary>
            /// <param name="o">对象</param>
            /// <returns>json字符串</returns>
            public static string SerializeObject(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return json;
            }

            /// <summary>
            /// 解析JSON字符串生成对象实体
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
            /// <returns>对象实体</returns>
            public static T DeserializeJsonToObject<T>(string json) where T : class
            {
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(json);
                object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
                T t = o as T;
                return t;
            }

            /// <summary>
            /// 解析JSON数组生成对象实体集合
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
            /// <returns>对象实体集合</returns>
            public static List<T> DeserializeJsonToList<T>(string json) where T : class
            {
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(json);
                object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
                List<T> list = o as List<T>;
                return list;
            }

            /// <summary>
            /// 反序列化JSON到给定的匿名对象.
            /// </summary>
            /// <typeparam name="T">匿名对象类型</typeparam>
            /// <param name="json">json字符串</param>
            /// <param name="anonymousTypeObject">匿名对象</param>
            /// <returns>匿名对象</returns>
            public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
            {
                T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
                return t;
            }
    }
