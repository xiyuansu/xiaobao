using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.app.pay.wxpay
{
    public class WxConfig
    {
        public static string AppId =SiteHelper.GetAppsetString("wx_AppId");

        public static string MchId = SiteHelper.GetAppsetString("wx_MchId");

        public static string PartnerKey = SiteHelper.GetAppsetString("wx_PayKey");
    }
}