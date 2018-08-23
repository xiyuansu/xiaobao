using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 删除常用地址
    /// </summary>
    public class DelOftenAddressByID : IApiAction2
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
                int num = am.DelAddressByID(addressid);
                if (num > 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "地址删除成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "地址删除失败");
                }
            }
        }
    }
}