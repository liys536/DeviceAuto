using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceAuto
{
    public class Util
    {
        public static Boolean loginsession;
        public static string sqlwhere;
        public static void SetLoginsession(Boolean success, string dateauth)
        {
            loginsession = success;
            sqlwhere = dateauth;
        }
    }
}