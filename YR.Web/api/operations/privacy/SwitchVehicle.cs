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
    /// 开关车辆
    /// </summary>
    public class SwitchVehicle : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;
        private string opr = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["VID"] == null || res["Opr"] == null || res["UID"].ToString().Trim().Length <= 0 || res["VID"].ToString().Trim().Length <= 0 || res["Opr"].ToString().Trim().Length <= 0)
            {
                Hashtable result = new Hashtable();
                result["ErrCode"] = "00";//服务器异常
                return SiteHelper.GetJsonFromHashTable(result, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vid = res["VID"].ToString().Trim();
                //opr,0:开1:关 
                opr = res["Opr"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                Hashtable ht = vm.GetVehicleInfoByID(vid);
                if (ht == null)
                {
                    Hashtable result = new Hashtable();
                    result["ErrCode"] = "01";//车辆未找到
                    return SiteHelper.GetJsonFromHashTable(result, "faild", "未找到相关车辆信息");
                }
                else
                {
                    //客户占用车辆时禁止操作
                    if (ht["USESTATE"].ToString() == VehicleUseState.Order.GetHashCode().ToString() || ht["USESTATE"].ToString() == VehicleUseState.Reservation.GetHashCode().ToString())
                    {
                        Hashtable result = new Hashtable();
                        result["ErrCode"] = "02";//车辆已被占用
                        return SiteHelper.GetJsonFromHashTable(result, "faild", "指令发送失败,车辆已被客户占用");
                    }

                    string gpsnum = SiteHelper.GetHashTableValueByKey(ht, "VehicleGPSNum");
                    bool isSuccess = false;
                    if (opr == "0")
                        isSuccess =vm.OpenVehicle(vid);
                    else
                        isSuccess =vm.CloseVehicle(vid);
                    if (!isSuccess)
                    {
                        Hashtable result = new Hashtable();
                        result["ErrCode"] = "04";//指令发送失败
                        return SiteHelper.GetJsonFromHashTable(result, "faild", "指令发送失败");
                    }
                    else
                    {
                        Hashtable vht = new Hashtable();
                        vht["ID"] = vid;
                        vht["UseState"] = opr == "0" ? 4 : 1;
                        vm.AddOrEditVehicleInfo(vht, vid);

                        OPUserManager hum = new OPUserManager();
                        Hashtable userOperateHT = new Hashtable();
                        userOperateHT["ID"] = CommonHelper.GetGuid;
                        userOperateHT["VehicleID"] = vid;
                        userOperateHT["UserID"] = uid;
                        userOperateHT["OperateType"] = opr == "0" ? 1 : 2;
                        userOperateHT["OperateTypeText"] = opr == "0" ? "开车" : "锁车";
                        userOperateHT["Remark"] = "";
                        userOperateHT["OperateTime"] = SiteHelper.GetWebServerCurrentTime();
                        hum.AddUserOperate(userOperateHT);

                        return SiteHelper.GetJsonFromHashTable(null, "success", "指令发送成功");
                    }
                }
            }
        }
    }
}