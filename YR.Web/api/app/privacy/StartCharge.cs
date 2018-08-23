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
    /// 充电桩开始充电
    /// </summary>
    public class StartCharge : IApiAction2
    {
        private string uid = string.Empty;

        private string chargingPointID = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["ChargingPointID"] == null || res["ChargingPointID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                chargingPointID = res["ChargingPointID"].ToString().Trim();
                ChargingPointsManager cpm = new ChargingPointsManager();
                ChargingOrdersManager com = new ChargingOrdersManager();
                Hashtable ht = cpm.GetChargingPointByID(chargingPointID);
                if (ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到充电桩数据");
                }
                else
                {
                    string imei = SiteHelper.GetHashTableValueByKey(ht, "IMEI");
                    bool isSuccess = cpm.GPSRemoteControlLock(imei, false);
                    if (!isSuccess)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "充电桩启动失败，请稍后重试");
                    }
                    else
                    {
                        Hashtable param = new Hashtable();
                        param["ID"] = CommonHelper.GetGuid;
                        param["OrderNum"] = SiteHelper.GenerateOrderNum();
                        param["ChargingPointID"] = chargingPointID;
                        param["UserID"] = uid;
                        param["PayState"] = OrderPayState.NotPay.GetHashCode();
                        param["BeginTime"] = SiteHelper.GetWebServerCurrentTime();
                        param["CreateTime"] = SiteHelper.GetWebServerCurrentTime();
                        bool result = com.NewOrder(param);
                        if (result)
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "success", "充电桩启动成功");
                        }
                        else
                        {
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "充电桩启动失败，请稍后重试");
                        }
                    }
                }

            }
        }

    }
}