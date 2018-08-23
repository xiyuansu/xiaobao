using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 用户修改银行卡
    /// </summary>
    public class UseModifyBankCard : IApiAction2
    {
        private string uid = string.Empty;
        private string accountname = string.Empty;
        private string account = string.Empty;
        private string openbank = string.Empty;
        private string bankname = string.Empty;
        private string bankid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["AccountName"] == null || res["Account"] == null || res["BankName"] == null || res["OpenBank"] == null ||
              res["BankCardID"] == null || res["UID"].ToString().Trim().Length <= 0 || res["AccountName"].ToString().Trim().Length <= 0 ||
               res["Account"].ToString().Trim().Length <= 0 || res["BankName"].ToString().Trim().Length <= 0
               || res["OpenBank"].ToString().Trim().Length <= 0 || res["BankCardID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                accountname = res["AccountName"].ToString().Trim();
                account = res["Account"].ToString().Trim();
                bankname = res["BankName"].ToString().Trim();
                openbank = res["OpenBank"].ToString().Trim();
                bankid = res["BankCardID"].ToString().Trim();
                Hashtable bankcard = new Hashtable();
                bankcard["ID"] = bankid;
                bankcard["UserID"] = uid;
                bankcard["AccountName"] = accountname;
                bankcard["Account"] = account;
                bankcard["BankName"] = bankname;
                bankcard["OpenBank"] = openbank;
                UserBankCardManager ubcm = new UserBankCardManager();
                bool isSuccess = ubcm.AddOrEditUserBankCardInfo(bankcard, bankid);
                if (isSuccess)
                {
                    Hashtable userbankcard = ubcm.GetUserBankCard(uid);
                    if (userbankcard == null)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "会员银行卡信息保存失败");
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(userbankcard, "success", "会员银行卡信息保存成功", "UserBankCardInfo");
                    }
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "会员银行卡信息保存失败");
                }
            }
        }

    }
}