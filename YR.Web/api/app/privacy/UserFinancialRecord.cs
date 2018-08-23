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
    /// 会员用车模块财务流水记录
    /// </summary>
    public class UserFinancialRecord : IApiAction2
    {
        private string uid = string.Empty;
        private int currentpage = 1;
        private int pagenum = 10;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["CurrentPage"] == null || res["PageNum"] == null
                || res["UID"].ToString().Trim().Length <= 0 || res["CurrentPage"].ToString().Trim().Length <= 0 ||
                res["PageNum"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    pagenum = int.Parse(res["PageNum"].ToString().Trim());
                    currentpage = int.Parse(res["CurrentPage"].ToString().Trim());
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }
                UserFinancialManager ufm = new UserFinancialManager();
                DataTable dt = ufm.GetUserFinancialInfoByUser(uid, currentpage, pagenum);
                foreach (DataRow dr in dt.Rows)
                {
                    string changetype = dr["ChangesType"].ToString().Trim();
                    if (dr["ChangesAmount"] == null || dr["ChangesAmount"].ToString().Trim().Length <= 0)
                    {
                        dr["ChangesAmount"] = "0";
                    }
                    else
                    {
                        if (changetype == UserFinancialChangesType.Recharge.GetHashCode().ToString() || changetype == UserFinancialChangesType.PlatAdd.GetHashCode().ToString())
                        {
                            dr["ChangesAmount"] = Math.Abs(decimal.Parse(dr["ChangesAmount"].ToString())).ToString();
                        }
                        else if (changetype == UserFinancialChangesType.Consumption.GetHashCode().ToString() || changetype == UserFinancialChangesType.PlatMinus.GetHashCode().ToString())
                        {
                            dr["ChangesAmount"] = (-Math.Abs(decimal.Parse(dr["ChangesAmount"].ToString()))).ToString();
                        }
                    }
                }
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "没有数据了");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", dt, "UserFinancialInfo");
                }
            }
        }

    }
}