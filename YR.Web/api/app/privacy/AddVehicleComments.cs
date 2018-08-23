using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 提交评论
    /// </summary>
    public class AddVehicleComments : IApiAction2
    {
        private string uid = string.Empty;
        private string vehicleid = string.Empty;
        private string usernickname = string.Empty;
        private string content = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;
            if (res["UID"] == null || res["VehicleID"] == null ||
                res["UID"].ToString().Trim().Length <= 0 || res["VehicleID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                if (res["UserNickName"] == null || res["UserNickName"].ToString().Trim().Length <= 0)
                {
                    usernickname = "匿名用户";
                }
                else
                {
                    usernickname = res["UserNickName"].ToString().Trim();
                }
                if (res["Content"] != null && res["Content"].ToString().Trim().Length > 0)
                {
                    content = res["Content"].ToString().Trim();
                }
                uid = res["UID"].ToString().Trim();
                vehicleid = res["VehicleID"].ToString().Trim();
                Hashtable comment = new Hashtable();
                comment["ID"] = CommonHelper.GetGuid;
                comment["VehicleID"] = vehicleid;
                comment["UserID"] = uid;
                comment["UserNickName"] = usernickname;
                comment["CommentsContent"] = content;
                comment["CommentsTime"] = SiteHelper.GetWebServerCurrentTime();
                comment["State"] = VehicleCommentsState.NewSubmit.GetHashCode();
                if (res["Client"] != null && "iOS" == res["Client"].ToString())
                {
                    comment["ClientType"] = "iOS";
                }
                else
                {
                    comment["ClientType"] = "Android";
                }
                VehicleCommentsLogManager vclm = new VehicleCommentsLogManager();
                bool isSuccess = vclm.AddOrEditVehicleCommentsLogInfo(comment, null);
                if (isSuccess)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "success", "评论保存成功，感谢您的参与。");
                }
                else
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "评论保存失败");
                }
            }
        }
    }
}