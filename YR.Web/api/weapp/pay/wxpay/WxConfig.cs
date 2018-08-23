using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.weapp.pay.wxpay
{
    public class WxConfig
    {
        public static string AppId = SiteHelper.GetAppsetString("weapp_AppId");

        public static string MchId = SiteHelper.GetAppsetString("weapp_MchId");

        public static string PartnerKey = SiteHelper.GetAppsetString("weapp_PayKey");

        public static string AppSecret = SiteHelper.GetAppsetString("weapp_AppSecret");

    }
}