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
    /// 获取网点信息列表
    /// </summary>
    public class GetParkingList : IApiAction2
    {
        private string uid = string.Empty;
        private int pageNum = int.MaxValue;
        private int currentPage = 1;
        string keyword = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["PageNum"] == null || res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"] == null || res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    pageNum = int.Parse(res["PageNum"].ToString().Trim());
                    currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                    if (res["Keyword"] != null && res["Keyword"].ToString().Trim().Length > 0)
                        keyword = res["Keyword"].ToString().Trim();
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                UserInfoManager uim = new UserInfoManager();
                ServiceAreaManager areaManager = new ServiceAreaManager();
                DataTable parkings = areaManager.GetParkingList(keyword, currentPage, pageNum);
                if (parkings == null || parkings.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", parkings, "GetParkingList");
                }
            }
        }
    }
}