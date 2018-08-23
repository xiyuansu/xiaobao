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
    /// 车辆解除异常
    /// </summary>
    public class VehicleUnAbnormal : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;
        private string remark = string.Empty;

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
                remark = "";
                if (res["Remark"] != null)
                    remark = res["Remark"].ToString().Trim();

                VehicleManager vm = new VehicleManager();
                bool isSuccess = vm.VehileUnAbnormal(uid, 2, vid, remark);
                if (!isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "车辆解除异常操作失败");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "车辆解除异常操作成功");
                }
            }
        }
    }
}