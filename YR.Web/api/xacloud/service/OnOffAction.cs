using YR.Common.DotNetCode;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;

namespace YR.Web.api.xacloud.service
{
    /// <summary>
    /// 上下线通知
    /// {"cmd","OnOff","imei":"12312312","type":"1"}
    /// imei:盒子号,type:1下线,2上线
    /// </summary>
    public class OnOffAction : IApiAction
    {
        private static Log Logger = LogFactory.GetLogger(typeof(OnOffAction));

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            try
            {
                string jsonData = params_ht["json"].ToString();
                dynamic jsonObj = DynamicJson.Parse(jsonData);

                string imei = "";
                if (jsonObj.IsDefined("imei"))
                    imei = jsonObj.imei;
                string type = "";
                if (jsonObj.IsDefined("type"))
                    type = jsonObj.type;

                Logger.Debug("上下线通知:" + imei + ",参数:" + jsonData);

                resp.Code = "0";
                return resp;
            }
            catch
            {
                resp.Code = "1";
                return resp;
            }
        }
    }
}