using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace YR.Web.api.baojia.service
{
    /// <summary>
    /// 获取企业基础信息
    /// </summary>
    public class GetCompanyInfo: IApiAction
    {

        public ApiResp Execute(System.Collections.Hashtable params_ht)
        {
            ApiResp resp = new ApiResp();
            resp.code = "-1";

            string client_id = params_ht["client_id"].ToString();
            long timestamp = long.Parse(params_ht["timestamp"].ToString());
            string sign = params_ht["sign"].ToString();

            List<GetCompanyInfo_Item> data = new List<GetCompanyInfo_Item>();

            GetCompanyInfo_Item item = new GetCompanyInfo_Item();

            SysSettingManager settingManager=new SysSettingManager();
            DictManager dictManager = new DictManager();
            item.insurance_amount = 0.00;
            item.insurance_url = "";
            item.return_in_fee = "";
            string outServiceAreaFee = settingManager.GetValueByKey("OutServiceAreaFee");
            if(outServiceAreaFee.StartsWith("+"))
                item.return_out_fee = string.Format("超出运营区域将加收{0}元调度费", outServiceAreaFee.Substring(1,outServiceAreaFee.Length-1));
            else
                item.return_out_fee = string.Format("超出运营区域将加收{0}倍作为调度费", outServiceAreaFee.Substring(1, outServiceAreaFee.Length - 1));
            item.call_tel=settingManager.GetValueByKey("ServiceTel");
            DataTable city_dt = dictManager.GetDictList("03");
            string open_city="";
            if(city_dt!=null && city_dt.Rows.Count>0)
            {
                foreach(DataRow dr in city_dt.Rows)
                {
                    if(open_city.Length>0 && !open_city.EndsWith("|"))
                        open_city+="|";
                    open_city+=dr["Name"].ToString();
                }
            }
            item.open_city=open_city;
            item.timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            data.Add(item);
            
            resp.code = "0";
            resp.msg = "成功";
            resp.data = data;

            return resp;
        }
    }

    public class GetCompanyInfo_Item
    {
        public string call_tel;

        public double insurance_amount;

        public string insurance_url;

        public string return_in_fee;

        public string return_out_fee;

        public string open_city;

        public long timestamp;
    }
}