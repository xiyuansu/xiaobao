using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Data;
using YR.Web.api.api_class;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 得到车辆列表
    /// </summary>
    public class GetVehicleList : IApiAction2
    {
        private string uid = string.Empty;
        private int useState = 0;
        private int vehicleState = 0;
        private int pageNum = 30;
        private int currentPage = 1;
        private int lowBattery = 0;
        private int chargeState = 0;
        private int abnormalState = 0;
        private string parkingID = string.Empty;
        private int sumCount = 0;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UseState"] == null || res["VehicleState"] == null ||
                res["PageNum"] == null || res["PageNum"].ToString().Trim().Length <= 0 ||
                res["CurrentPage"] == null || res["CurrentPage"].ToString().Trim().Length <= 0 ||
                res["UID"].ToString().Trim().Length <= 0 || res["UseState"].ToString().Trim().Length <= 0 || res["VehicleState"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                try
                {
                    uid = res["UID"].ToString().Trim();
                    useState = int.Parse(res["UseState"].ToString().Trim());
                    vehicleState = int.Parse(res["VehicleState"].ToString().Trim());
                    currentPage = int.Parse(res["CurrentPage"].ToString().Trim());
                    pageNum = int.Parse(res["PageNum"].ToString().Trim());
                    if (res["LowBattery"] != null)
                        lowBattery = int.Parse(res["LowBattery"].ToString().Trim());
                    if (res["ChargeState"] != null)
                        chargeState = int.Parse(res["ChargeState"].ToString().Trim());
                    if (res["ParkingID"] != null)
                        parkingID = res["ParkingID"].ToString().Trim();
                    if (res["AbnormalState"] != null)
                        abnormalState = int.Parse(res["AbnormalState"].ToString().Trim());
                }
                catch
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "数据格式不正确");
                }

                VehicleManager vm = new VehicleManager();
                OrdersManager om = new OrdersManager();
                ServiceAreaManager areaManager = new ServiceAreaManager();
                DataTable vehicles = areaManager.GetVehiclesByParkingID(uid, parkingID, useState, vehicleState, lowBattery, chargeState, abnormalState, currentPage, pageNum, out sumCount);
                if (vehicles == null || vehicles.Rows.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "亲，没有数据了");
                }
                else
                {
                    vehicles.Columns.Add("OrderUserName");
                    vehicles.Columns.Add("OrderUserTel");
                    vehicles.Columns.Add("HeadPortrait");
                    vehicles.Columns.Add("OrderState");
                    vehicles.Columns.Add("OrderTime");
                    vehicles.Columns.Add("UserType");
                    vehicles.Columns.Add("AbnormalState");
                    vehicles.Columns.Add("AlarmType");
                    vehicles.Columns.Add("AlarmTime");
                    foreach (DataRow dr in vehicles.Rows)
                    {
                        LatLng latlng = new LatLng(double.Parse(dr["Latitude"].ToString()), double.Parse(dr["Longitude"].ToString()));
                        latlng = SiteHelper.TransformFromWGSToGCJ(latlng);
                        dr["Longitude"] = latlng.longitude;
                        dr["Latitude"] = latlng.latitude;

                        dr["ThumbImageUrl"] = SiteHelper.GetFileServer() + dr["ThumbImageUrl"].ToString();
                        decimal electricity = 0;
                        decimal.TryParse(dr["Electricity"].ToString(), out electricity);
                        int dianliang = Decimal.ToInt32(electricity);
                        dr["Electricity"] = dianliang + "%";

                        dr["AbnormalState"] = 0;
                        Hashtable abht = null;// vm.GetAbnormalVehicle(dr["ID"].ToString());
                        if (abht != null && abht.Keys.Count > 0)
                        {
                            dr["AbnormalState"] = 1;
                            dr["AlarmType"] = SiteHelper.GetHashTableValueByKey(abht, "AlarmType");
                            dr["AlarmTime"] = SiteHelper.GetHashTableValueByKey(abht, "AlarmTime");
                        }
                        Hashtable order_ht = om.GetLatestUserByVehicleID(dr["ID"].ToString());
                        if (order_ht != null && order_ht.Keys.Count > 0)
                        {
                            dr["OrderUserName"] = order_ht["REALNAME"];
                            dr["OrderUserTel"] = order_ht["BINDPHONE"];
                            //dr["HeadPortrait"] = SiteHelper.GetFileServer() + order_ht["HEADPORTRAIT"];
                            dr["HeadPortrait"] = "";
                            dr["OrderState"] = order_ht["ORDERSTATE"];
                            dr["UserType"] = order_ht["USERTYPE"];
                            dr["OrderTime"] = order_ht["CREATETIME"];
                        }
                        else
                        {
                            dr["OrderUserName"] = "";
                            dr["OrderUserTel"] = "";
                            dr["HeadPortrait"] = "";
                            dr["OrderState"] = "";
                            dr["UserType"] = "";
                            dr["OrderTime"] = "";
                        }
                    }
                    
                    return SiteHelper.GetJsonFromDataTable2(vehicles, "success", "获取数据成功", "GetVehicleList", sumCount);
                }
            }
        }
    }
}