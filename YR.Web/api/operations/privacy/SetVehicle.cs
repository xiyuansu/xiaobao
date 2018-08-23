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
    /// 更新车辆信息
    /// </summary>
    public class SetVehicle : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;
        private string contentType = string.Empty;
        private string content = string.Empty;

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
                //contentType,修改类型：01车辆名称，02车牌号，03车型，04车辆平台,05车辆模块编码，06车辆状态，07车架号
                if (res["ContentType"] != null && res["ContentType"].ToString().Trim().Length > 0)
                    contentType = res["ContentType"].ToString().Trim();
                if (res["Content"] != null && res["Content"].ToString().Trim().Length > 0)
                    content = res["Content"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vid);
                if (vehicle_ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到相关车辆信息");
                }

                Hashtable ht = new Hashtable();
                if (!string.IsNullOrEmpty(contentType))
                {
                    switch (contentType)
                    {
                        case "01":
                            ht["Name"] = content;
                            break;
                        case "02":
                            ht["VehicleName"] = content;
                            ht["LicenseNumber"] = content;
                            break;
                        case "03":
                            ht["Model"] = content;
                            break;
                        case "04":
                            ht["PlatformId"] = content;
                            break;
                        case "05":
                            ht["VehicleGPSNum"] = content;
                            break;
                        case "06":
                            ht["VehicleState"] = content;
                            break;
                        case "07":
                            ht["VehicleNum"] = content;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (res["Name"] != null && res["Name"].ToString().Trim().Length > 0)
                        ht["Name"] = res["Name"].ToString().Trim();
                    if (res["VehicleName"] != null && res["VehicleName"].ToString().Trim().Length > 0)
                    {
                        ht["VehicleName"] = res["VehicleName"].ToString().Trim();
                        ht["LicenseNumber"] = res["VehicleName"].ToString().Trim();
                    }
                    if (res["Model"] != null && res["Model"].ToString().Trim().Length > 0)
                        ht["Model"] = res["Model"].ToString().Trim();
                    if (res["PlatformId"] != null && res["PlatformId"].ToString().Trim().Length > 0)
                        ht["PlatformId"] = res["PlatformId"].ToString().Trim();
                    if (res["VehicleGPSNum"] != null && res["VehicleGPSNum"].ToString().Trim().Length > 0)
                        ht["VehicleGPSNum"] = res["VehicleGPSNum"].ToString().Trim();
                    if (res["VehicleState"] != null && res["VehicleState"].ToString().Trim().Length > 0)
                        ht["VehicleState"] = res["VehicleState"].ToString().Trim();
                    if (res["VehicleNum"] != null && res["VehicleNum"].ToString().Trim().Length > 0)
                        ht["VehicleNum"] = res["VehicleNum"].ToString().Trim();
                }
                bool isSuccess = vm.AddOrEditVehicleInfo(ht, vehicle_ht["ID"].ToString());
                if (!isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "修改车辆信息失败");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "修改车辆信息成功");
                }
            }
        }
    }
}