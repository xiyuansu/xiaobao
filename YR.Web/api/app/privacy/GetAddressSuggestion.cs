using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 得到建议地址
    /// </summary>
    public class GetAddressSuggestion : IApiAction2
    {
        private string uid = string.Empty;
        private string addresskeyword = string.Empty;
        private string region = string.Empty;
        private string location = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["AddressKeyword"] == null || res["Region"] == null || res["Location"] == null
                || res["UID"].ToString().Trim().Length <= 0 || res["AddressKeyword"].ToString().Trim().Length <= 0
                || res["Region"].ToString().Trim().Length <= 0 || res["Location"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                addresskeyword = res["AddressKeyword"].ToString().Trim();
                region = res["Region"].ToString().Trim();
                location = res["Location"].ToString().Trim();
                return SiteHelper.GetAddressSuggestion(addresskeyword, region, location);
            }
        }
    }
}