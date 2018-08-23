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
    /// 获取会员押金金额
    /// </summary>
    public class GetDepositMoney : IApiAction2
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
                SysSettingManager settingManager = new SysSettingManager();
                string depositMoney = settingManager.GetValueByKey("GoldDepositAmount");
                if (string.IsNullOrEmpty(depositMoney))
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据");
                }
                else
                {
                    Hashtable result = new Hashtable();
                    result["deposit"] = depositMoney;
                    return SiteHelper.GetJsonFromHashTable(result, "success", "获取数据成功", "data");
                }
            }
        }
    }
}