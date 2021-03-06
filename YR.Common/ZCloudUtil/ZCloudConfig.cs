﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using YR.Common.DotNetConfig;

namespace YR.Common.ZCloudUtil
{
    public class ZCloudConfig
    {
        public static string charset = "UTF-8";

	    public static string sign_key = "sign";

        public static string md5_key =ConfigHelper.GetAppSettings("ZCloud_KEY");

        public static string api_url =ConfigHelper.GetAppSettings("ZCloud_URL");
    }
}