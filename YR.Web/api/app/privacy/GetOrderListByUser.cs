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
    /// 根据用户得到订单列表
    /// </summary>
    public class GetOrderListByUser : IApiAction2
    {
        private string uid = string.Empty;

        private int pagenum = 10;

        private int currentpage = 1;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["PageNum"] == null || res["CurrentPage"] == null ||
                res["UID"].ToString().Trim().Length <= 0 || res["PageNum"].ToString().Trim().Length <= 0 || res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                try
                {
                    pagenum = int.Parse(res["PageNum"].ToString().Trim());
                    currentpage = int.Parse(res["CurrentPage"].ToString().Trim());
                }
                catch (Exception e1)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "参数格式不正确");
                }
                OrdersManager om = new OrdersManager();
                DataTable dt = om.GetOrdersInfoByUser(uid, currentpage, pagenum);
                if (res["Client"] != null && "iOS" == res["Client"].ToString())
                {
                    //苹果系统请求时进行类型转换，新版本审核后可删除
                    DataTable dt1 = dt.Clone();
                    dt1.Columns["OrderNum"].DataType = typeof(string);
                    dt1.Columns["Mileage"].DataType = typeof(string);
                    dt1.Columns["Minutes"].DataType = typeof(string);
                    dt1.Columns["PayMoney"].DataType = typeof(string);
                    foreach (DataRow dr in dt.Rows)
                        dt1.Rows.Add(dr.ItemArray);
                    dt = dt1;
                }
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据了，亲");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", dt, "OrdersInfo");
                }
            }
        }
    }
}