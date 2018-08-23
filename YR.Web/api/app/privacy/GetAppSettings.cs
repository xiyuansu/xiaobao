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

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取app公用参数信息接口
    /// </summary>
    public class GetAppSettings : IApiAction2
    {
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                SysSettingManager settingManager = new SysSettingManager();
                //DataTable dt = settingManager.GetSettings("0102");
                DataTable dt = settingManager.GetSettings("");
                if (dt != null && dt.Rows.Count > 0)
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", dt, "GetAppSettings");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了");
                }
            }
        }
    }
}