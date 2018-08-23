using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;
using System.Data;
using YR.Common.DotNetJson;
using YR.Web.api.api_class;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 获取车辆报警信息列表
    /// </summary>
    public class GetVehicleAlarmList : IApiAction2
    {
        private string uid = string.Empty;
        private int pageNum = 30;
        private int currentPage = 1;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null ||
                res["PageNum"] == null ||
                res["CurrentPage"] == null ||
                res["UID"].ToString().Trim().Length <= 0 ||
                res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                    pageNum = int.Parse(res["PageNum"].ToString().Trim());
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                VehicleManager vm = new VehicleManager();
                DataTable dt = vm.GetVehicleAlarmList(uid, currentPage, pageNum);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了");
                }
                else
                {
                    return JsonHelper.DataTableToJson("success", "获取数据成功", dt, "GetVehicleAlarmList");
                }
            }
        }
    }
}