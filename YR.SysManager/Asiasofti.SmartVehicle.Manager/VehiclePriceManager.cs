using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YR.Common;
using YR.DataBase;
using YR.Busines;
using System.Collections;
using YR.Common.DotNetCode;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.Common.DotNetData;
using System.Net;
using YR.Common.DotNetBean;
using System.Configuration;
using System.Xml;


namespace Asiasofti.SmartVehicle.Manager
{
    public class VehiclePriceManager
    {
        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehiclePriceRule", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int Delete(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehiclePriceRule", "ID", _key, ht);
        }

        /// <summary>
        /// 通过ID获取信息
        /// </summary>
        /// <param name="vehicleID">规则ID</param>
        /// <returns></returns>
        public Hashtable GetPriceByID(string ruleID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_VehiclePriceRule", "ID", ruleID);
        }

        /// <summary>
        /// 列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select *,dbo.GetCityName(CityID) CityName,dbo.GetModelName(ModelID) ModelName from YR_VehiclePriceRule where DeleteMark!=0");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Asc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取所有价格规则信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPriceInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_VehiclePriceRule where DeleteMark!=0 order by CreateTime");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 根据车型及所在城市获取计费规则
        /// </summary>
        /// <param name="cityid"></param>
        /// <param name="modelid"></param>
        /// <returns></returns>
        public DataTable GetPriceInfo(string cityid, string modelid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from YR_VehiclePriceRule where DeleteMark!=0 and CityID='{0}' and ModelID='{1}'", cityid, modelid);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 获取订单结算信息
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public Hashtable GetOrderSettlement(string orderid)
        {
            Hashtable result = new Hashtable();

            UserInfoManager userManager = new UserInfoManager();
            OrdersManager ordersManager = new OrdersManager();
            VehicleManager vehicleManager = new VehicleManager();
            SysSettingManager settingManager = new SysSettingManager();
            Hashtable order_ht = ordersManager.GetOrderInfoByID(orderid);

            string orderState = SiteHelper.GetHashTableValueByKey(order_ht, "OrderState");
            string userID = SiteHelper.GetHashTableValueByKey(order_ht, "UserID");
            if (OrderState.Valid.GetHashCode().ToString() == orderState)
            {   
                #region 未结算订单

                int minutes = 0;
                float mileage = 0.00f;
                decimal minutePrice = 0.00m;
                decimal kmPrice = 0.00m;
                decimal minPrice = 0.00m;
                decimal maxPrice = 0.00m;
                decimal totalMoney = 0.00m;

                int orderFreeMinutes = 0;
                int.TryParse(settingManager.GetValueByKey("OrderFreeMinutes"), out orderFreeMinutes);
                double orderFreeKm = 0;
                double.TryParse(settingManager.GetValueByKey("OrderFreeKm"), out orderFreeKm);

                float.TryParse(SiteHelper.GetHashTableValueByKey(order_ht, "Mileage"), out mileage);

                Hashtable vehicle_ht = vehicleManager.GetVehicleInfoByID(SiteHelper.GetHashTableValueByKey(order_ht, "VehicleID"));
                Hashtable user_ht = userManager.GetUserInfoByUserID(userID);
                DataTable price_dt = GetPriceInfo(SiteHelper.GetHashTableValueByKey(order_ht, "CityID"), SiteHelper.GetHashTableValueByKey(vehicle_ht, "Model"));
                if (price_dt != null && price_dt.Rows.Count > 0)
                {
                    decimal.TryParse(price_dt.Rows[0]["MinutePrice"].ToString(), out minutePrice);
                    decimal.TryParse(price_dt.Rows[0]["KmPrice"].ToString(), out kmPrice);
                    decimal.TryParse(price_dt.Rows[0]["MinPrice"].ToString(), out minPrice);
                    decimal.TryParse(price_dt.Rows[0]["MaxPrice"].ToString(), out maxPrice);
                }
                minutes = ordersManager.GetOrderBillingTime(SiteHelper.GetHashTableValueByKey(order_ht, "OrderNum"));
                if (minutes > 0)
                {
                    //totalMoney = minutePrice * minutes + kmPrice * (decimal)mileage;
                    totalMoney = minutePrice * (minutes - (minutes > orderFreeMinutes ? orderFreeMinutes : 0)) + kmPrice * (decimal)(mileage - (mileage > orderFreeKm ? orderFreeKm : 0));
                    if (totalMoney < minPrice)
                        totalMoney = minPrice;
                    if (totalMoney > maxPrice)
                        totalMoney = maxPrice;
                }

                //用车5分钟以内或骑行100米以内还车都不收费
                if (minutes < orderFreeMinutes || mileage < orderFreeKm)
                {
                    totalMoney = 0.00m;
                }

                result["MinutePrice"] = minutePrice;
                result["KmPrice"] = kmPrice;
                result["Minutes"] = minutes;
                result["TotalMoney"] = totalMoney;

                #endregion
            }
            else
            {
                #region 已结算订单

                result["MinutePrice"] = SiteHelper.GetHashTableValueByKey(order_ht, "MinutePrice");
                result["KmPrice"] = SiteHelper.GetHashTableValueByKey(order_ht, "KMPrice");
                result["Minutes"] = SiteHelper.GetHashTableValueByKey(order_ht, "Minutes");
                result["TotalMoney"] = SiteHelper.GetHashTableValueByKey(order_ht, "TotalMoney");

                #endregion
            }

            return result;
        }

        /// <summary>
        /// 根据用户和车辆id获取车辆计费规则
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="vehicleid"></param>
        /// <returns></returns>
        public Hashtable GetVehiclePrice(string vehicleid)
        {
            Hashtable result = new Hashtable();

            UserInfoManager userManager = new UserInfoManager();
            VehicleManager vehicleManager = new VehicleManager();
            SysSettingManager settingManager = new SysSettingManager();
            Hashtable vehicle_ht = vehicleManager.GetVehicleInfoByID(vehicleid);
            if (vehicle_ht != null)
            {
                decimal minutePrice = 0.00m;
                decimal kmPrice = 0.00m;
                decimal minPrice = 0.00m;
                decimal maxPrice = 0.00m;

                DataTable price_dt = GetPriceInfo(SiteHelper.GetHashTableValueByKey(vehicle_ht, "CityID"), SiteHelper.GetHashTableValueByKey(vehicle_ht, "Model"));
                if (price_dt != null && price_dt.Rows.Count > 0)
                {
                    decimal.TryParse(price_dt.Rows[0]["MinutePrice"].ToString(), out minutePrice);
                    decimal.TryParse(price_dt.Rows[0]["KmPrice"].ToString(), out kmPrice);
                    decimal.TryParse(price_dt.Rows[0]["MinPrice"].ToString(), out minPrice);
                    decimal.TryParse(price_dt.Rows[0]["MaxPrice"].ToString(), out maxPrice);
                }
                result["MinutePrice"] = minutePrice;
                result["KmPrice"] = kmPrice;
                result["MinPrice"] = minPrice;
                result["MaxPrice"] = maxPrice;
            }

            return result;
        }

    }
}
