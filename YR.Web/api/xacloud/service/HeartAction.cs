using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using YR.Common.DotNetCode;
using YR.Web.api.api_class;

namespace YR.Web.api.xacloud.service
{
    /// <summary>
    /// 心跳信息
    /// {"dumpEle":"0.57","csq":"55","sign":"15fb9447444f97645311a0c71261396d","cmd":"heart","time":1498881287873,"voltage":"49","carId":"865067025282255"}
    /// </summary>
    public class HeartAction : IApiAction
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
                string csq = "";
                if (jsonObj.IsDefined("csq"))
                    csq = jsonObj.csq;
                string voltage = "";
                if (jsonObj.IsDefined("voltage"))
                    voltage = jsonObj.voltage;
                string dumpEle = "";
                if (jsonObj.IsDefined("dumpEle"))
                    dumpEle = jsonObj.dumpEle;

                VehicleManager vm = new VehicleManager();
                Hashtable vehicle_ht = vm.GetVehicleInfoByGPSNum(carId);
                if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
                {
                    string strLastUpdateTime = SiteHelper.GetHashTableValueByKey(vehicle_ht, "LastUpdateTime");
                    int diffSecond = 5;
                    if (!string.IsNullOrEmpty(strLastUpdateTime))
                    {
                        DateTime lastUpdateTime = DateTime.Parse(strLastUpdateTime);
                        diffSecond = SiteHelper.ExecDateDiffSecond(lastUpdateTime, DateTime.Now);
                    }
                    if (diffSecond > 3)
                    {
                        string vid = vehicle_ht["ID"].ToString();
                        Hashtable ht = new Hashtable();
                        if (!string.IsNullOrEmpty(dumpEle))
                        {
                            double power = -1;
                            double.TryParse(dumpEle, out power);
                            if (power >= 0 && power <= 1)
                            {
                                ht["Electricity"] = (int)(power * 100);
                            }
                        }
                        ht["LastUpdateTime"] = DateTime.Now;
                        bool result = false;
                        if (ht.Keys.Count > 0)
                        {
                            result = vm.AddOrEditVehicleInfo(ht, vid);
                        }
                        resp.Code = result ? "0" : "1";
                    }
                    else
                    {
                        resp.Code = "3";
                    }
                }
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