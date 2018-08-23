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
    /// 上报故障
    /// </summary>
    public class UserReportFault : IApiAction2
    {
        private string uid = string.Empty;
        private string vehicleid = string.Empty;
        private string faulttype = string.Empty;
        private string images = string.Empty;
        private const int maxImageNum = 9;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["VehicleID"] == null || res["FaultType"] == null
                || res["UID"].ToString().Trim().Length <= 0 || res["VehicleID"].ToString().Trim().Length <= 0 || res["FaultType"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    vehicleid = res["VehicleID"].ToString().Trim();
                    faulttype = res["FaultType"].ToString().Trim();
                    if (res["Images"] != null)
                        images = res["Images"].ToString().Trim();
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                VehicleManager vm = new VehicleManager();
                OPUserManager userManager = new OPUserManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicleid);
                if (vehicle_ht == null || vehicle_ht.Keys.Count == 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到车辆信息,故障报修提交失败");
                }

                Hashtable user_ht = userManager.GetUserInfoByUserID(uid);
                string path = SiteHelper.GetAppsetString("OtherImage");
                string FaultImages = "";
                if (!string.IsNullOrEmpty(images))
                {
                    string[] datas = images.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (datas.Length > maxImageNum)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "报修图片数量过多，最多可上传" + maxImageNum + "张图片");
                    }
                    foreach (string data in datas)
                    {
                        string imgPath = BitmapHelper.toUpload(data);
                        FaultImages += imgPath + ";";
                    }
                    if (FaultImages.Length > 0)
                        FaultImages = FaultImages.Substring(0, FaultImages.Length - 1);
                }

                Hashtable vehiclefault = new Hashtable();
                vehiclefault["ID"] = CommonHelper.GetGuid;
                vehiclefault["VehicleName"] = SiteHelper.GetHashTableValueByKey(vehicle_ht, "LicenseNumber");
                vehiclefault["SubmitUserID"] = uid;
                vehiclefault["SubmitUserName"] = SiteHelper.GetHashTableValueByKey(user_ht, "UserName"); ;
                vehiclefault["TriggerType"] = (int)VehicleFaultTriggerType.User;
                vehiclefault["FaultType"] = faulttype;
                vehiclefault["Remark"] = "";
                vehiclefault["FaultDoState"] = (int)VehicleFaultDoState.Undisposed;
                vehiclefault["CreateTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                if (FaultImages.Length > 0)
                {
                    vehiclefault["FaultImages"] = FaultImages;
                }
                VehicleFaultManager vfm = new VehicleFaultManager();
                bool isSuccess = vfm.AddOrEditVehicleFaultInfo(vehiclefault, null);
                if (isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "故障报修提交成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "故障报修提交失败");
                }

            }
        }
    }
}