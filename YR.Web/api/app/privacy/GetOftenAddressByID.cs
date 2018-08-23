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
    /// 根据id获取地址信息
    /// </summary>
    public class GetOftenAddressByID : IApiAction2
    {
        private string uid = string.Empty;
        private string addressid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["AddressID"] == null || res["UID"].ToString().Trim().Length <= 0 || res["AddressID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                addressid = res["AddressID"].ToString().Trim();
                AddressManager am = new AddressManager();
                Hashtable address = am.GetAddressByID(addressid);
                if (address != null)
                {
                    return SiteHelper.GetJsonFromHashTable(address, "success", "获取数据成功", "AddressInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "不存在信息记录");
                }
            }
        }
    }
}