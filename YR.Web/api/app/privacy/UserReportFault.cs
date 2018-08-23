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
    /// 用户上报车辆故障
    /// </summary>
    public class UserReportFault : IApiAction2
    {
        private string uid = string.Empty;
        private string vehiclename = string.Empty;
        private string faulttype = string.Empty;
        private string remark = string.Empty;
        private const int maxImageNum = 9;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["VehicleName"] == null || res["VehicleName"].ToString().Trim().Length <= 0 || 
                res["FaultType"] == null || res["FaultType"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vehiclename = res["VehicleName"].ToString().Trim();
                faulttype = res["FaultType"].ToString().Trim();
                if (res["Remark"] != null)
                    remark = res["Remark"].ToString().Trim();

                string FaultImages = "";
                string imageDatas = string.Empty;
                if (res["Images"] != null)
                    imageDatas = res["Images"].ToString().Trim();

                if (!string.IsNullOrEmpty(imageDatas))
                {
                    string[] datas = imageDatas.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (datas.Length > maxImageNum)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "上报图片数量过多，最多可上传" + maxImageNum + "张图片");
                    }
                    foreach (string data in datas)
                    {
                        string imgPath = BitmapHelper.toUpload(data);
                        FaultImages += imgPath + ";";
                    }
                    if (FaultImages.Length > 0)
                        FaultImages = FaultImages.Substring(0, FaultImages.Length - 1);
                }

                VehicleFaultManager vfm = new VehicleFaultManager();
                UserInfoManager uim = new UserInfoManager();
                Hashtable userht= uim.GetUserInfoByUserID(uid);

                Hashtable vehiclefault = new Hashtable();
                vehiclefault["ID"] = CommonHelper.GetGuid;
                vehiclefault["VehicleName"] = vehiclename;
                vehiclefault["SubmitUserID"] = uid;
                vehiclefault["SubmitUserName"] = SiteHelper.GetHashTableValueByKey(userht, "RealName") ;
                vehiclefault["TriggerType"] = (int)VehicleFaultTriggerType.User;
                vehiclefault["FaultType"] = faulttype;
                vehiclefault["Remark"] = remark;
                vehiclefault["FaultImages"] = FaultImages;
                vehiclefault["FaultDoState"] = (int)VehicleFaultDoState.Undisposed;
                vehiclefault["CreateTime"] = SiteHelper.GetWebServerCurrentTime().ToString();

                bool isSuccess = vfm.AddOrEditVehicleFaultInfo(vehiclefault, null);
                if (isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "上报车辆故障成功");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "上报车辆故障失败");
                }
            }
        }

    }
}