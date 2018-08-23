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
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取车辆故障类型列表
    /// </summary>
    public class GetVehicleFaultType : IApiAction2
    {
        private string uid;

        public string Execute(Hashtable res)
        {
            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();

                ListApiResp resp = new ListApiResp();
                resp.Code = "-1";
                resp.Message = "";

                DictManager dictManager = new DictManager();
                DataTable dt= dictManager.GetDictList("05");
                if (dt!=null && dt.Rows.Count>0)
                {
                    List<GetVehicleFaultTypeItem> list = new List<GetVehicleFaultTypeItem>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        GetVehicleFaultTypeItem item = new GetVehicleFaultTypeItem();
                        item.id = dr["id"].ToString();
                        item.name = dr["name"].ToString();
                        list.Add(item);
                    }
                    resp.List = list;
                    resp.Total = list.Count;
                    resp.Code = "0";
                }

                return JsonConvert.SerializeObject(resp);
            }
        }
    }

    /// <summary>
    /// 接口GetVehicleFaultType返回数据列表项
    /// </summary>
    class GetVehicleFaultTypeItem
    {
        public string id;

        public string name;
    }
}