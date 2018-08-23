using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 根据会员得到会员绑定的银行卡信息
    /// </summary>
    public class GetUseBankCardByUser : IApiAction2
    {
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                UserBankCardManager ubcm = new UserBankCardManager();
                Hashtable bankcard = ubcm.GetUserBankCard(uid);
                if (bankcard == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(bankcard, "success", "获取数据成功", "UserBankCardInfo");
                }
            }
        }
    }
}