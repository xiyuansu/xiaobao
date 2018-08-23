using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;
using System.Data;
using YR.Common.DotNetData;
using YR.Web.api.api_class;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    public class GetUserInfo : IApiAction2
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
                OPUserManager userManager = new OPUserManager();
                DataTable dt = userManager.GetUserInfoByUid(uid);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "获取用户信息失败");
                }
                else
                {
                    Hashtable huser = DataTableHelper.DataRowToHashTable(dt.Rows[0]);
                    DataTable dt_parking= userManager.GetUserParkingList(uid);
                    DataRow dr= dt_parking.NewRow();
                    dr["ID"] = System.Guid.Empty.ToString();
                    dr["ThisName"] ="停车点外";
                    dt_parking.Rows.InsertAt(dr, 0);
                    huser["PARKLIST"] = dt_parking;
                    //huser["PARKLIST"] = userManager.GetUserParkingList(uid);
                    return SiteHelper.GetJsonFromHashTable2(huser, "success", "获取数据成功", "UserInfo");
                }
            }
        }
    }
}