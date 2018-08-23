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
    public class GetRechargeRules : IApiAction2
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

                RechargeRules rechargeRules = new RechargeRules();
                DataTable rules = rechargeRules.GetAvailableRules(null);
                if (res["Client"] != null && "iOS" == res["Client"].ToString())
                {
                    //苹果系统请求时进行类型转换，新版本审核后可删除
                    DataTable dt = rules.Clone();
                    dt.Columns["ChargeMoney"].DataType = typeof(string);
                    dt.Columns["PresentMoney"].DataType = typeof(string);
                    foreach (DataRow dr in rules.Rows)
                        dt.Rows.Add(dr.ItemArray);
                    rules = dt;
                }
                if (rules == null || rules.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据为空");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", rules, "RechargeRules");
                }
            }
        }
    }
}