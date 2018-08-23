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
    /// 寻车
    /// </summary>
    public class LookingVehicle : IApiAction2
    {
        private string uid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                OrdersManager om = new OrdersManager();
                VehicleManager vm = new VehicleManager();
                Hashtable ht = om.GetUserCurrentOrderVehicleGPSNum(uid);
                if (ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您没有车辆无法寻车");
                }
                else
                {
                    string vid = SiteHelper.GetHashTableValueByKey(ht, "VID");
                    string gpsnum = SiteHelper.GetHashTableValueByKey(ht, "VehicleGPSNum");
                    bool isSuccess = vm.FindVehicle(vid);
                    if (!isSuccess)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "指令发送失败");
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "success", "指令发送成功");
                    }
                }
            }
        }

    }
}