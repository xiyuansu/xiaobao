/*
 * 车辆数据操作类
 * 作者：SJ
 * 时间：2015-05-10
 */
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
    /// <summary>
    /// 车辆管理 
    /// </summary>
    public class VehicleManager
    {
        /// <summary>
        /// 添加或修改车辆信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditVehicleInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Vehicles", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除车辆信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_Vehicles", "ID", _key, ht);
        }

        /// <summary>
        /// 通过车辆ID获取车辆信息
        /// </summary>
        /// <param name="vehicleID">车辆ID</param>
        /// <returns></returns>
        public Hashtable GetVehicleInfoByID(string vehicleID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_Vehicles", "ID", vehicleID);
        }

        public Hashtable GetVehicleInfoByLicenseNumber(string licenseNumber)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_Vehicles", "LicenseNumber", licenseNumber);
        }

        /// <summary>
        /// 通过车辆ID获取车辆信息
        /// </summary>
        /// <param name="vehicleID">车辆ID</param>
        /// <returns></returns>
        public Hashtable GetVehicleInfoByID(string vehicleID, string weidu, string jingdu)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,dbo.fnGetDistance(Latitude,Longitude,'" + weidu + "','" + jingdu + "') as Distance FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1");
            return DataFactory.SqlDataBase().GetHashtableBySQL(sql, "a.ID", vehicleID);
        }

        public Hashtable GetVehicleInfoByName(string vehicleName, string weidu, string jingdu)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,dbo.fnGetDistance(Latitude,Longitude,'" + weidu + "','" + jingdu + "') as Distance FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and a.Name=@Name");
            SqlParam param = new SqlParam();
            param.FieldName = "@Name";
            param.FiledValue = vehicleName;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        public Hashtable GetVehicleInfoByGPSNum(string gpsnum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_Vehicles where DeleteMark=1 and VehicleGPSNum=@VehicleGPSNum");
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleGPSNum";
            param.FiledValue = gpsnum;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        public Hashtable GetVehicleIByGPSNum(string gpsnum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_Vehicles where DeleteMark=1 and UseState=1 and VehicleGPSNum=@VehicleGPSNum");
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleGPSNum";
            param.FiledValue = gpsnum;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 车辆列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetVehicleInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,dbo.GetCityName(a.cityid) VehicleCityName,b.UserName,c.Platform,isnull(datediff(n,LastUpdateTime,Getdate()),9999) diffminutes,vm.Name modelName from YR_Vehicles a left join YR_UserInfo b  on a.UserID=b.ID left join YR_VehiclePlatform c on a.PlatformId=c.ID left join  YR_VehicleModel vm on a.Model=vm.ID where a.DeleteMark!=0 ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 根据经纬度分页获取某个公里范围内的可用车辆
        /// </summary>
        /// <param name="km">某个公里范围内</param>
        /// <param name="weidu">纬度</param>
        /// <param name="jingdu">经度</param>
        /// <param name="currentPage">当前第几页</param>
        /// <param name="pageNum">每页数据条数</param>
        /// <returns></returns>
        public DataTable GetNearVehicles(double km, string weidu, string jingdu, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int pageIndex = currentPage;
            int pageSize = pageNum;
            int count = 0;
            IList<SqlParam> IList_param = new List<SqlParam>();
            strSql.Append("select * from (");
            strSql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,dbo.fnGetDistance(Latitude,Longitude,'" + weidu + "','" + jingdu + "') as Distance,LastUpdateTime FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and Longitude is not null and Latitude is not null and UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and convert(float,Electricity)>=30 and LastUpdateTime>Dateadd(n,-5, Getdate())");
            strSql.Append(") t where Distance<=" + km + "");
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Distance", "asc", pageIndex, pageSize, ref count);
        }
        public DataTable GetAllVehicles(int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            //strSql.Append("select ID,Name,KMPrice,MinutesPrice,Latitude,Longitude,Address,Electricity,Range,ThumbImageUrl from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl from  yr_vehicles where Longitude is not null and Latitude is not null and UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and DeleteMark=1 and LastUpdateTime>Dateadd(n,-5, Getdate()))   as Vehicle) as Vehicles where RowNumber between " + start + " and " + end + " order by LastUpdateTime asc");
            strSql.Append("select ID,Name,KMPrice,MinutesPrice,Latitude,Longitude,Address,Electricity,Range,ThumbImageUrl from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl from  yr_vehicles where Longitude is not null and Latitude is not null and UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and DeleteMark=1)   as Vehicle) as Vehicles where RowNumber between " + start + " and " + end + " order by LastUpdateTime asc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 根据用户获取车辆信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Hashtable GetVehicleInfoByUser(string oid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select top 1 * from yr_vehicles where USERID=@UserID order by CreateTime asc");
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = oid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 获取所有车辆信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllVehicleInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from yr_vehicles where DeleteMark!=0 order by CreateTime asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        public DataTable GetInUseVehicles()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select ID,Name,VehicleName,VehicleGPSNum,UseState,VehicleState,LockState from YR_Vehicles where UseState=2");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 根据车辆ID查询使用情况
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetVehicleUseInfoByID(string id)
        {
            StringBuilder strSql = new StringBuilder();
            //strSql.Append(@"select yr_vehicles.ID as VID,yr_vehicles.VehicleGPSNum,yr_vehicles.UseState,yr_vehicles.VehicleState,yr_vehicles.LockState,yr_orders.UserID,YR_UserInfo.RealName,YR_UserInfo.BindPhone,yr_vehicles.[Name],yr_vehicles.Model,yr_vehicles.Longitude,yr_vehicles.Latitude,yr_vehicles.Address,yr_vehicles.ExpectRange,yr_vehicles.Electricity,yr_vehicles.[Range],yr_vehicles.ExpectHighestSpeed,yr_vehicles.CurrentSpeed,yr_orders.StartTime,yr_orders.OrderState,datediff(MINUTE,yr_orders.StartTime, GETDATE() ) as StartMinutes,yr_orders.KMPrice,yr_orders.MinutePrice,yr_orders.Mileage,yr_orders.Minutes,yr_orders.CreateTime,yr_orders.ID as orderid,yr_orders.OrderNum,yr_orders.RentDuration,yr_orders.TotalMoney,yr_orders.SettlementMoney from yr_orders left join yr_vehicles on yr_orders.vehicleid = yr_vehicles.id left join YR_UserInfo on yr_orders.UserID=YR_UserInfo.ID where yr_vehicles.ID=@ID and yr_vehicles.UseState=2 and yr_orders.orderstate=1");
            strSql.Append(@"select yr_vehicles.ID as VID,yr_vehicles.VehicleGPSNum,yr_vehicles.UseState,yr_vehicles.VehicleState,yr_vehicles.LockState,yr_orders.UserID,view_OrderUser.RealName,view_OrderUser.BindPhone,view_OrderUser.OutOrderNo,yr_vehicles.[Name],yr_vehicles.Model,yr_vehicles.Longitude,yr_vehicles.Latitude,yr_vehicles.Address,yr_vehicles.ExpectRange,yr_vehicles.Electricity,yr_vehicles.[Range],yr_vehicles.ExpectHighestSpeed,yr_vehicles.CurrentSpeed,yr_orders.StartTime,yr_orders.OrderState,datediff(MINUTE,yr_orders.StartTime, GETDATE() ) as StartMinutes,yr_orders.KMPrice,yr_orders.MinutePrice,yr_orders.Mileage,yr_orders.Minutes,yr_orders.CreateTime,yr_orders.ID as orderid,yr_orders.OrderNum,yr_orders.RentDuration,yr_orders.TotalMoney,yr_orders.SettlementMoney from yr_orders left join yr_vehicles on yr_orders.vehicleid = yr_vehicles.id left join view_OrderUser on yr_orders.ID=view_OrderUser.ID where yr_vehicles.ID=@ID and yr_vehicles.UseState=2 and yr_orders.orderstate=1");
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = id;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        public DataTable GetVehicles(string name, int? vstate, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,LicenseNumber,VehicleGPSNum,KMPrice,MinutesPrice,Latitude,Longitude,Address,Electricity,Range,Mileage,ThumbImageUrl,datediff(n,LastUpdateTime,Getdate()) diffminutes,isnull(TodayMileage,0.00) TodayMileage,LastUpdateTime from (select ROW_NUMBER() OVER (ORDER BY LastUpdateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl,(SELECT TOP 1 YR_VehicleMileage.Mileage From YR_VehicleMileage WHERE YR_VehicleMileage.VehicleId=yr_vehicles.ID and RecordDate=convert(varchar(10),getdate(),120)) AS TodayMileage from  yr_vehicles where DeleteMark=1");
            if (vstate.HasValue)
            {
                strSql.Append(" and VehicleState=" + vstate + "");
            }
            if (!string.IsNullOrEmpty(name))
                strSql.Append(" and (Name like '%" + name + "%' or LicenseNumber like '%" + name + "%')");
            strSql.Append(") as Vehicle) as Vehicles where RowNumber between " + start + " and " + end);
            strSql.Append(" order by LastUpdateTime desc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        public DataTable GetAllVehicleInfo(string name, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,KMPrice,MinutesPrice,Latitude,Longitude,Address,Electricity,Range,Mileage,ThumbImageUrl,datediff(n,LastUpdateTime,Getdate()) diffminutes,isnull(TodayMileage,0.00) TodayMileage,LastUpdateTime from (select ROW_NUMBER() OVER (ORDER BY LastUpdateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl,(SELECT TOP 1 YR_VehicleMileage.Mileage From YR_VehicleMileage WHERE YR_VehicleMileage.VehicleId=yr_vehicles.ID and RecordDate=convert(varchar(10),getdate(),120)) AS TodayMileage from  yr_vehicles where DeleteMark=1) as Vehicle) as Vehicles where RowNumber between " + start + " and " + end);
            if (!string.IsNullOrEmpty(name))
                strSql.Append(" and VehicleName like '%" + name + "%'");
            strSql.Append(" order by LastUpdateTime desc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 更新车辆状态（锁定，灯）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="oprStr"></param>
        /// <param name="gpsID"></param>
        /// <returns></returns>
        public bool UpdateLockLightStateByGPSID(int type, string oprStr, string gpsID)
        {
            string fieldStr = type == VehicleControlType.Lock.GetHashCode() ? "LockState='" + oprStr + "'" : "LightState='" + oprStr + "'";

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE YR_Vehicles SET " + fieldStr + ",LastUpdateTime = GETDATE() WHERE VehicleGPSNum='" + gpsID + "'");
            int iNum = DataFactory.SqlDataBase().ExecuteBySql(sb);
            return iNum > 0 ? true : false;
        }

        /// <summary>
        /// 获取车辆未处理的报警信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public Hashtable GetAbnormalVehicle(string vehicleID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT * FROM YR_VehicleAlarms where AlarmStatus=0 and VehicleID=@VehicleID");
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleID";
            param.FiledValue = vehicleID;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 解除车辆报警
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="uidtype"></param>
        /// <param name="vid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool VehileUnAbnormal(string uid, int uidtype, string vid, string remark)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"update YR_VehicleAlarms set AlarmStatus=1,OperatorID='" + uid + "',OperatorType=" + uidtype + ",OperateRemark='" + remark + "',OperateTime=getdate() where AlarmStatus=0 and VehicleID='" + vid + "'");
            return DataFactory.SqlDataBase().ExecuteBySql(strSql) > 0 ? true : false;
        }

        /// <summary>
        /// 获取车辆报警信息列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetVehicleAlarmList(string uid, int currentPage, int pageNum)
        {
            int count = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT YR_VehicleAlarms.ID,VehicleID,YR_Vehicles.Name,AlarmType,AlarmTime,AlarmStatus,OperatorID,OperatorType,OperateRemark,OperateTime FROM YR_VehicleAlarms join YR_Vehicles on YR_VehicleAlarms.vehicleid=YR_Vehicles.id where parkingID in(SELECT ParkingID FROM YR_OPUserParking where UserID='" + uid + "') and AlarmStatus=0");
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), null, "AlarmTime", "desc", currentPage, pageNum, ref count);
        }

        public Hashtable GetVehicleInfoByIDORNumber(string id)
        {
            StringBuilder strSql = new StringBuilder();
            //strSql.Append(@"select ID,Name,LicenseNumber,VehicleName,Model, dbo.GetModelName(Model) ModelName,PlatformId,dbo.GetPlatformName(PlatformId) PlatformName,VehicleGPSNum,KMPrice,MinutesPrice,Latitude,Longitude,Address,VehicleColor,Electricity,Range,UseState,VehicleState,ChargeState,LastUpdateTime,ParkingID,dbo.GetParkingName(ID) ParkingName from  yr_vehicles where (DeleteMark!=0 or DeleteMark is null) and (ID=@ID or LicenseNumber=@ID)");
            strSql.Append("SELECT a.ID,a.Name,LicenseNumber,VehicleName,Model,b.Name ModelName,PlatformId,dbo.GetPlatformName(a.PlatformId) PlatformName,VehicleGPSNum,VehicleNum,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,Longitude,Latitude,Address,VehicleColor,Electricity,Range,UseState,VehicleState,ChargeState,CurrentSpeed,LastUpdateTime,ParkingID,dbo.GetParkingName(a.ID) ParkingName FROM YR_Vehicles a left join YR_VehicleModel b on a.Model=b.ID left join YR_VehiclePriceRule c on a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and (a.ID=@ID or a.Name=@ID or a.LicenseNumber=@ID)");
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = id;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 获取车辆当前充电信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public Hashtable GetVehicleChargingInfo(string vehicleID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT ID,VehicleID,BeginTime,endTime,BeginPower,EndPower FROM YR_VehiclesChargeLog where vehicleid=@VehicleID and endtime is null");
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleID";
            param.FiledValue = vehicleID;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 获取车辆行驶轨迹
        /// </summary>
        /// <returns></returns>
        public DataTable GetVehicleTrace(string vehicleid, string orderid, DateTime? begin, DateTime? end)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select *,dbo.TransformFromWGSToGCJ_LNG([Latitude],[Longitude]) 'AMAP_longitude',dbo.TransformFromWGSToGCJ_LAT([Latitude],[Longitude]) 'AMAP_latitude' from YR_VehicleTrace where Latitude is not null and Longitude is not null");
            if (!string.IsNullOrEmpty(vehicleid))
            {
                strSql.Append(string.Format(" and VehicleID='{0}' ", vehicleid));
            }
            if (!string.IsNullOrEmpty(orderid))
            {
                strSql.Append(string.Format(" and OrderID='{0}' ", orderid));
            }
            else
            {
                if (!begin.HasValue)
                {
                    begin = DateTime.Now.AddDays(-1);
                }
                strSql.Append(string.Format(" and RecordTime>='{0}' ", begin.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                if (!end.HasValue)
                {
                    end = DateTime.Now;
                }
                strSql.Append(string.Format(" and RecordTime<='{0}' ", end.Value.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            strSql.Append(" order by RecordTime asc ");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 获取车辆速度行驶轨迹
        /// </summary>
        /// <returns></returns>
        public DataTable GetSpeedVehicleTrace(string vehicleid, DateTime begin, DateTime end)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select VehicleID,Speed,RecordTime from YR_VehicleTrace");
            strSql.Append(string.Format(" where VehicleID='{0}'  and  Speed>0", vehicleid));
            strSql.Append(string.Format(" and RecordTime>='{0}' ", begin.ToString("yyyy-MM-dd HH:mm:ss")));
            strSql.Append(string.Format(" and RecordTime<='{0}' ", end.ToString("yyyy-MM-dd HH:mm:ss")));
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 车辆操控日志
        /// </summary>
        /// <param name="operate_type">操作类型：01开车，02锁车，03寻车，04开座锁</param>
        /// <param name="gpsID">车辆gps编号</param>
        /// <param name="result">操控返回结果：0失败，1成功</param>
        /// <returns></returns>
        public bool VehicleOperateLog(string operate_type, string vehicleid, bool result, string returnResult)
        {
            string gpsNum = "";
            string longitude = "";
            string latitude = "";
            string speed = "0";
            string power = "0";
            string lockState = "";
            Hashtable vehicle_ht = GetVehicleInfoByID(vehicleid);
            if (vehicle_ht != null)
            {
                gpsNum = SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum");
                longitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Longitude");
                latitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Latitude");
                speed = SiteHelper.GetHashTableValueByKey(vehicle_ht, "CurrentSpeed");
                power = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Electricity");
                lockState = SiteHelper.GetHashTableValueByKey(vehicle_ht, "LockState");
            }
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();
            //StringBuilder sbAddLog = new StringBuilder();
            //sbAddLog.Append("INSERT INTO YR_VehicleDriveLog(ID,VehicleID,OrderID,GPSNum,Longitude,Latitude,Address,Direction,Speed,Power,LockState,LightState,RecordTime,DriveStyle,IsStart,IsEnd,OprResult,ReturnResult) " +
            // "SELECT convert(varchar(50),newid()),YR_Vehicles.ID,YR_Orders.ID,VehicleGPSNum,Longitude,Latitude,null,null,CurrentSpeed,Electricity,LockState,LightState,getdate(),@DriveStyle,null,null,@OprResult,@ReturnResult " +
            // "FROM YR_Vehicles left join YR_Orders on YR_Vehicles.ID=YR_Orders.VehicleID and OrderState=1 where YR_Vehicles.DeleteMark=1 and YR_Vehicles.ID=@ID");
            StringBuilder sbAddLog = new StringBuilder();
            sbAddLog.Append("INSERT INTO YR_VehicleDriveLog(ID,VehicleID,GPSNum,Longitude,Latitude,Speed,Power,LockState,RecordTime,DriveStyle,IsStart,IsEnd,OprResult,ReturnResult) VALUES(convert(varchar(50),newid()),@VehicleID,@GPSNum,@Longitude,@Latitude,@Speed,@Power,@LockState,getdate(),@DriveStyle,null,null,@OprResult,@ReturnResult)");
            SqlParam[] parmAddLog = new SqlParam[] {
                new SqlParam("@VehicleID",vehicleid),
                new SqlParam("@GPSNum",gpsNum),
                new SqlParam("@Longitude",longitude),
                new SqlParam("@Latitude",latitude),
                new SqlParam("@Speed",speed),
                new SqlParam("@Power",power),
                new SqlParam("@LockState",lockState),
                new SqlParam("@DriveStyle",operate_type),
                new SqlParam("@OprResult",result?1:0),
                new SqlParam("@ReturnResult",returnResult),
                new SqlParam("@ID",vehicleid)
            };
            sqls.Add(sbAddLog);
            objs.Add(parmAddLog);

            if (result)
            {
                if (operate_type == "01")
                {
                    //开车
                    StringBuilder sbUpdateVehicle = new StringBuilder();
                    sbUpdateVehicle.Append("UPDATE YR_Vehicles SET LockState='unlock' WHERE ID=@ID");
                    SqlParam[] parmUpdateVehicle = new SqlParam[] {
                        new SqlParam("@ID",vehicleid)
                    };
                    sqls.Add(sbUpdateVehicle);
                    objs.Add(parmUpdateVehicle);
                }
                else if (operate_type == "02")
                {
                    //锁车
                    StringBuilder sbUpdateVehicle = new StringBuilder();
                    sbUpdateVehicle.Append("UPDATE YR_Vehicles SET LockState='lock' WHERE ID=@ID");
                    SqlParam[] parmUpdateVehicle = new SqlParam[] {
                        new SqlParam("@ID",vehicleid)
                    };
                    sqls.Add(sbUpdateVehicle);
                    objs.Add(parmUpdateVehicle);
                }
                else if (operate_type == "03")
                {
                    //寻车
                }
                else if (operate_type == "04")
                {
                    //开座锁
                }
                else
                {
                    //其它...
                }
            }
            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray()) ? true : false;
        }

        /// <summary>
        /// 获取无单且开锁的车辆列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetNoOrderUnlockVehicles()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,Name,Latitude,Longitude,Electricity,CurrentSpeed,VehicleGPSNum from yr_vehicles where UseState=1 and LockState='unlock' and DeleteMark=1 ");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 获取无单的车辆列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetNoOrderVehicles()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select ID,Name,Electricity,CurrentSpeed,VehicleGPSNum from yr_vehicles where UseState=1 and DeleteMark=1 and ID not in(select VehicleID from YR_Orders where OrderState<>0 and (OrderState=1");
            strSql.Append(string.Format(" or EndTime>='{0}'))", DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss")));
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        #region 车辆控制

        public IVehicleController GetController(string vid)
        {
            Hashtable vehicle_ht = GetVehicleInfoByID(vid);
            string platform_id = SiteHelper.GetHashTableValueByKey(vehicle_ht, "PlatformId").ToUpper();
            IVehicleController vehicleController = null;
            switch (platform_id)
            {
                /*case "0EF1FB75-9792-4E2E-8D21-6AA4302E49A5":
                    vehicleController = new SZVehicleController();
                    break;
                case "0EF1FB75-9792-4E2E-8D21-6AA4302E49A6":
                    vehicleController = new SZ2VehicleController();
                    break;
                case "F064C683-DB9C-4DF4-9208-474BE935CCC9":
                    vehicleController = new ZYBoxVehicleController();
                    break;
                case "F064C683-DB9C-4DF4-9208-474BE966CCC9":
                    vehicleController = new ZYBoxVehicleController();
                    break;*/
                case "DCA83FF6-DBB6-41F1-BE45-FDCE7C87AA25":
                    vehicleController = new XiaoAnVehicleController();
                    break;
                default:
                    break;
            }
            return vehicleController;
        }

        /// <summary>
        /// 开车指令
        /// </summary>
        /// <returns></returns>
        public bool OpenVehicle(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.Open(vid, out returnResult);
                ///list/item("01", vid, result, returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 锁车指令
        /// </summary>
        /// <returns></returns>
        public bool CloseVehicle(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.Close(vid, out returnResult);
                VehicleOperateLog("02", vid, result, returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 寻车指令
        /// </summary>
        /// <param name="gpsID"></param>
        /// <returns></returns>
        public bool FindVehicle(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.Find(vid, out returnResult);
                VehicleOperateLog("03", vid, result,returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 开座垫指令
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public bool OpenVehicleSeat(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.OpenSeat(vid, out returnResult);
                VehicleOperateLog("04", vid, result,returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 断电指令
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public bool PowerOff(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.PowerOff(vid, out returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 开电池锁指令
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public bool OpenBatteryLock(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.OpenBatteryLock(vid, out returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 关电池锁指令
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public bool CloseBatteryLock(string vid)
        {
            try
            {
                bool result = false;
                IVehicleController vehicleController = new XiaoAnVehicleController();
                string returnResult = "";
                result = vehicleController.CloseBatteryLock(vid, out returnResult);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

    }
}
