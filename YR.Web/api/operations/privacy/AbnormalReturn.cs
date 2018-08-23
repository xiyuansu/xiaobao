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

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 异常还车
    /// </summary>
    public class AbnormalReturn : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;
        private string returntype = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["VID"] == null || res["ReturnType"] == null || res["UID"].ToString().Trim().Length <= 0 || res["VID"].ToString().Trim().Length <= 0 || res["ReturnType"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vid = res["VID"].ToString().Trim();
                //异常还车类型:1收费，2不收费
                returntype = res["ReturnType"].ToString().Trim();

                VehicleManager vm = new VehicleManager();
                UserInfoManager uim = new UserInfoManager();
                OrdersManager om = new OrdersManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vid);
                if (vehicle_ht == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "未找到相关车辆信息");
                }
                Hashtable order_ht = om.GetLatestUserOrder(vehicle_ht["ID"].ToString());
                if (order_ht == null || order_ht.Keys.Count <= 0 || order_ht["ORDERSTATE"].ToString() != OrderState.Valid.GetHashCode().ToString())
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆当前未被任何用户占用");
                }
                bool isSuccess = false;
                if (returntype == "1")
                {
                    isSuccess = uim.AbnormalReturnVehicle(order_ht["USERID"].ToString(), true);
                }
                else if (returntype == "2")
                {
                    isSuccess = uim.AbnormalReturnVehicle(order_ht["USERID"].ToString(),false);
                }
                if (!isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "异常还车操作失败");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "异常还车操作成功");
                }
            }
        }
    }
}