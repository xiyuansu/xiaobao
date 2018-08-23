using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace YR.Common.ZCloudUtil
{
    public class ZCloudSignConfig
    {
        public static string charset = "UTF-8";

	    public static string sign_key = "sign";

        public static string md5_key = ConfigurationManager.AppSettings["ZCloud_KEY"].ToString().Trim();

        public static string api_url = ConfigurationManager.AppSettings["ZCloud_URL"].ToString().Trim();
    }
}