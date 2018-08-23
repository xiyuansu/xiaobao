using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;
using YR.Web.api.weapp.pay.wxpay;

namespace YR.Web.api.weapp.privacy
{
    /// <summary>
    /// code 换取 session_key
    /// </summary>
    public class JSCode2Session : IApiAction2
    {
        private string uid;

        private string jscode;

        public string Execute(Hashtable res)
        {
            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["js_code"] == null || res["js_code"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                jscode = res["js_code"].ToString().Trim();

                string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", WxConfig.AppId, WxConfig.AppSecret, jscode);
                string retstr = HttpUtil.Get(url, "", "utf-8");

                Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(retstr);
                if (jobj["openid"] == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", jobj["errmsg"].ToString());
                }
                else
                {
                    dynamic retObj = DynamicJson.Parse(retstr);
                    Hashtable result = new Hashtable();
                    result["openid"] = retObj.openid.ToString();
                    result["session_key"] = retObj.session_key.ToString();
                    return SiteHelper.GetJsonFromHashTable(result, "success", "获取数据成功", "UserInfo");
                }
            }
        }
    }
}