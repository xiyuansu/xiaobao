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

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    public class GetUserCoupons : IApiAction2
    {
        private string uid = string.Empty;

        private int currentpage = 1;

        private int pagenum = 10;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"] == null || res["CurrentPage"].ToString().Trim().Length <= 0 ||
                res["PageNum"] == null || res["PageNum"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                pagenum = int.Parse(res["PageNum"].ToString().Trim());
                currentpage = int.Parse(res["CurrentPage"].ToString().Trim());
                bool valid = false;
                if (res["Valid"] != null && res["Valid"].ToString().Trim().Length > 0)
                {
                    valid = res["Valid"].ToString().Trim() == "1" ? true : false;
                }
                UserCouponManager couponManager = new UserCouponManager();
                int count = 0;
                DataTable dt = couponManager.GetUserCoupons(uid, valid, currentpage, pagenum, ref count);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据了");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", dt, "GetUserCoupons");
                }
            }
        }
    }
}