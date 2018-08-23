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
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 车辆解绑网点
    /// </summary>
    public class VehicleUnBind : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["VID"] == null || res["VID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vid = res["VID"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vid);
                if (vehicle_ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到相关车辆信息");
                }
                Hashtable ht = new Hashtable();
                ht["VehicleID"] = vehicle_ht["ID"];
                ht["ParkingID"] = null;
                bool isSuccess = vm.AddOrEditVehicleInfo(ht, vehicle_ht["ID"].ToString());
                if (!isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "解绑车辆与网点失败");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "解绑车辆与网点成功");
                }
            }
        }
    }
}