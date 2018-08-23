using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.xacloud.service
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginAction : IApiAction
    {
        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            try
            {
                string jsonData = params_ht["json"].ToString();
                dynamic jsonObj = DynamicJson.Parse(jsonData);

                string carId = "";
                if (jsonObj.IsDefined("carId"))
                    carId = jsonObj.carId;
                
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