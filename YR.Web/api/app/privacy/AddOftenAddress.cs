using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 添加常用地址
    /// </summary>
    public class AddOftenAddress : IApiAction2
    {
        private string uid = string.Empty;
        private string shorttitle = string.Empty;
        private string address = string.Empty;
        private string jingdu = string.Empty;
        private string weidu = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;
            if (res["UID"] == null || res["ShortTitle"] == null || res["Address"] == null || res["Longitude"] == null || res["Latitude"] == null
                || res["UID"].ToString().Trim().Length <= 0 || res["ShortTitle"].ToString().Trim().Length <= 0
                || res["Address"].ToString().Trim().Length <= 0 || res["Longitude"].ToString().Trim().Length <= 0
                || res["Latitude"].ToString().Trim().Length <= 0)
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

                Hashtable hashadd = new Hashtable();
                hashadd["ID"] = CommonHelper.GetGuid;
                hashadd["UserID"] = uid;
                hashadd["Address"] = address;
                hashadd["ShortTitle"] = shorttitle;
                hashadd["Longitude"] = jingdu;
                hashadd["Latitude"] = weidu;
                AddressManager am = new AddressManager();
                bool isSuccess = am.AddOrEditAddressInfo(hashadd, null);
                if (isSuccess)
                {
                    Hashtable hashaddress = am.GetAddressByID(hashadd["ID"].ToString());
                    return SiteHelper.GetJsonFromHashTable(hashaddress, "success", "添加地址成功", "AddressInfo");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "添加地址失败");
                }
            }
        }
    }
}