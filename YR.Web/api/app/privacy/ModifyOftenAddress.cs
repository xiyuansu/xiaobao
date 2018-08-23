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
    /// 修改常用地址
    /// </summary>
    public class ModifyOftenAddress : IApiAction2
    {
        private string uid = string.Empty;
        private string shorttitle = string.Empty;
        private string address = string.Empty;
        private string jingdu = string.Empty;
        private string weidu = string.Empty;
        private string addressid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["ShortTitle"] == null || res["Address"] == null || res["Longitude"] == null || res["Latitude"] == null
               || res["AddressID"] == null || res["UID"].ToString().Trim().Length <= 0 || res["ShortTitle"].ToString().Trim().Length <= 0
                || res["Address"].ToString().Trim().Length <= 0 || res["Longitude"].ToString().Trim().Length <= 0
                || res["Latitude"].ToString().Trim().Length <= 0 || res["AddressID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                shorttitle = res["ShortTitle"].ToString().Trim();
                address = res["Address"].ToString().Trim();
                jingdu = res["Longitude"].ToString().Trim();
                weidu = res["Latitude"].ToString().Trim();
                addressid = res["AddressID"].ToString().Trim();

                Hashtable hashadd = new Hashtable();
                hashadd["ID"] = addressid;
                hashadd["UserID"] = uid;
                hashadd["Address"] = address;
                hashadd["ShortTitle"] = shorttitle;
                hashadd["Longitude"] = jingdu;
                hashadd["Latitude"] = weidu;
                AddressManager am = new AddressManager();
                bool isSuccess = am.AddOrEditAddressInfo(hashadd, hashadd["ID"].ToString());
                if (isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(hashadd, "success", "修改地址成功", "AddressInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "修改地址失败");
                }
            }
        }

    }
}