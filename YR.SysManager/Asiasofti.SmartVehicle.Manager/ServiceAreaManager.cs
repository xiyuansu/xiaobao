using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YR.Busines;
using System.Collections;
using YR.Common.DotNetCode;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 充电桩管理 
    /// </summary>
    public class ServiceAreaManager
    {
        public bool AddOrEditInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_ServiceArea", "ID", _key, ht);
        }
        public int DeleteByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_ServiceArea", "ID", _key, ht);
        }
        public Hashtable GetInfoByID(string areaID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_ServiceArea", "ID", areaID);
        }
        public DataTable GetServiceAreaByID(string key)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select * From YR_ServiceArea Where ID=@ID");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb, new SqlParam[] { new SqlParam("@ID", key) });
        }
        public DataTable GetInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select *,dbo.GetCityName(CityID) CityName,dbo.GetCityCode(CityID) CityCode from YR_ServiceArea where DeleteMark=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取附近停车网点列表
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable GetNearParking(double distance, string longitude, string latitude, int currentPage, int pageNum, ref int count)
        {
            IList<SqlParam> IList_param = new List<SqlParam>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,ThisName,Coordinates,Longitude,Latitude,Distance from (select *,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance from YR_ServiceArea where DeleteMark=1 and AreaType=2 and Status=1 and Longitude is not null and Latitude is not null and Coordinates is not null) as Parking where Distance<=" + distance);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Distance", "asc", currentPage, pageNum, ref count);
        }

        /// <summary>
        /// 根据城市代码获取车辆运营区域
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        public Hashtable GetServiceAreaByCityCode(string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_ServiceArea where DeleteMark=1 and AreaType=1 and Status=1 and Coordinates is not null and dbo.GetCityCode(CityID)=@CityCode");
            SqlParam param = new SqlParam();
            param.FieldName = "@CityCode";
            param.FiledValue = citycode;
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

        public DataTable GetServiceAreaListByCityCode(string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_ServiceArea where DeleteMark=1 and AreaType=1 and Status=1 and Coordinates is not null and dbo.GetCityCode(CityID)=@CityCode");
            SqlParam param = new SqlParam();
            param.FieldName = "@CityCode";
            param.FiledValue = citycode;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            return dt;
        }

        public DataTable GetServiceAreaList(string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_ServiceArea where DeleteMark=1 and AreaType=1 and Status=1 and Coordinates is not null");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, null);
            return dt;
        }

        public DataTable GetServiceAreaListByCityID(string cityID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_ServiceArea where DeleteMark=1 and AreaType=1 and Status=1 and Coordinates is not null and CityID=@CityID");
            SqlParam param = new SqlParam();
            param.FieldName = "@CityID";
            param.FiledValue = cityID;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            return dt;
        }

        public DataTable GetAreaList(int levelType, int parentId)
        {
            StringBuilder sql = new StringBuilder();
            SqlParam[] parms = null;
            if (levelType.Equals(1))
            {
                sql.Append("SELECT ID,Name,ParentId,ShortName,LevelType,CityCode,ZipCode,lng,Lat FROM YR_Area where LevelType=@LevelType");
                parms = new SqlParam[] {
                    new SqlParam("@LevelType",levelType)
                 };
            }
            else
            {
                sql.Append("SELECT ID,Name,ParentId,ShortName,LevelType,CityCode,ZipCode,lng,Lat FROM YR_Area where  LevelType=@LevelType and ParentId=@ParentId");
                parms = new SqlParam[] {
                    new SqlParam("@LevelType",levelType),
                     new SqlParam("@ParentId",parentId)
                 };
            }
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, parms);
            return dt;
        }

        public DataTable GetServiceAreaListByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM YR_ServiceArea where DeleteMark=1 and AreaType=1 and Status=1 and Coordinates is not null and CityID=(select CityID from YR_Vehicles where ID=@ID)");
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = id;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            return dt;
        }

        /// <summary>
        /// 根据车辆id获取车辆运营区域
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public Hashtable GetServiceAreaByVehicleID(string vid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ID,CityID,Coordinates,AreaType,ThisName,Longitude,Latitude,Status,DeleteMark,CreateTime from YR_ServiceArea where Status=1 and AreaType=1 and DeleteMark=1 and cityid in(select CityID FROM YR_Vehicles where id=@VehicleID)");
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleID";
            param.FiledValue = vid;
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
        /// 根据城市代码获取车辆停车网点
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        public DataTable GetParkingListByCityCode(string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ID,CityID,Coordinates,AreaType,ThisName,Longitude,Latitude,Address,Status,DeleteMark,CreateTime FROM YR_ServiceArea where DeleteMark=1 and AreaType=2 and Status=1 and Coordinates is not null and dbo.GetCityCode(CityID)=@CityCode");
            SqlParam param = new SqlParam();
            param.FieldName = "@CityCode";
            param.FiledValue = citycode;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            return dt;
        }

        public DataTable GetForbidListByCityCode(string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ID,CityID,Coordinates,AreaType,ThisName,Longitude,Latitude,Address,Status,DeleteMark,CreateTime FROM YR_ServiceArea where DeleteMark=1 and AreaType=3 and Status=1 and Coordinates is not null and dbo.GetCityCode(CityID)=@CityCode");
            SqlParam param = new SqlParam();
            param.FieldName = "@CityCode";
            param.FiledValue = citycode;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            return dt;
        }

        /// <summary>
        /// 获取车辆最近的停车点信息
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public Hashtable GetNearestParking(string longitude, string latitude, string vid)
        {
            IList<SqlParam> IList_param = new List<SqlParam>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 ID,ThisName,Coordinates,Longitude,Latitude,Distance from (select ID,ThisName,Coordinates,Longitude,Latitude,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance from YR_ServiceArea where DeleteMark=1 and AreaType=2 and Status=1 and Longitude is not null and Latitude is not null and cityid in(select CityID FROM YR_Vehicles where id='" + vid + "')) as Parking order by Distance asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        public DataTable GetNearestParkingList(string longitude, string latitude, string vid)
        {
            IList<SqlParam> IList_param = new List<SqlParam>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,ThisName,Coordinates,Longitude,Latitude,Distance from (select ID,ThisName,Coordinates,Longitude,Latitude,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance from YR_ServiceArea where DeleteMark=1 and AreaType=2 and Status=1 and Longitude is not null and Latitude is not null and cityid in(select CityID FROM YR_Vehicles where id='" + vid + "')) as Parking where Distance<5.00 order by Distance asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        public DataTable GetNearestForbidList(string longitude, string latitude, string vid)
        {
            IList<SqlParam> IList_param = new List<SqlParam>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,ThisName,Coordinates,Longitude,Latitude,Distance from (select ID,ThisName,Coordinates,Longitude,Latitude,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance from YR_ServiceArea where DeleteMark=1 and AreaType=3 and Status=1 and Longitude is not null and Latitude is not null and cityid in(select CityID FROM YR_Vehicles where id='" + vid + "')) as Parking where Distance<3.00 order by Distance asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        public DataTable GetVehiclesByParkingID(string userid, string parkingID, int use_state, int vehicle_state, int lowBattery, int chargeState, int abnormalState, int currentPage, int pageNum, out int count)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,LicenseNumber,KMPrice,MinutesPrice,Latitude,Longitude,Address,VehicleColor,Electricity,Range,ThumbImageUrl,UseState,VehicleState,ChargeState,LastUpdateTime,dbo.GetParkingName(ID) ParkingName");
            strSql.Append(" from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl from  yr_vehicles where (DeleteMark=1 or DeleteMark is null) ");
            strSql.Append(" and exists(SELECT 1 FROM YR_OPUserParking where UserID='" + userid + "' and (ParkingID=yr_vehicles.ParkingID or ParkingID is null))");
            if (!string.IsNullOrEmpty(parkingID) && Guid.Empty.ToString().CompareTo(parkingID) != 0)
                strSql.Append(" and parkingID='" + parkingID + "'");
            else if (Guid.Empty.ToString().CompareTo(parkingID) == 0)
                strSql.Append(" and parkingID is null");
            if (use_state > 0)
            {
                if (use_state == 1)
                    strSql.Append(" and (UseState=" + use_state + " and convert(float,Electricity)>=20)");
                else
                    strSql.Append(" and UseState in(2,3,4)");
            }
            if (vehicle_state > 0)
                strSql.Append(" and VehicleState=" + vehicle_state);
            if (lowBattery == 1)
                strSql.Append(" and convert(float,Electricity)<30");
            if (chargeState == 1)
                strSql.Append(" and ChargeState=1");
            if (abnormalState == 1)
                strSql.Append(" and ID in(SELECT VehicleID FROM YR_VehicleAlarms where AlarmStatus=0)");
            strSql.Append(" )as Vehicle) as Vehicles");
            count = Convert.ToInt32(DataFactory.SqlDataBase().GetObjectValue(new StringBuilder("Select Count(1) From (" + strSql.ToString() + ") As t")));
            strSql.Append(" where RowNumber between " + start + " and " + end + " order by Name asc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 根据关键字获取网点列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetParkingList(string keyword, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,CityID,Coordinates,AreaType,ThisName,Longitude,Latitude,Address,Status,DeleteMark,CreateTime from YR_ServiceArea where DeleteMark=1 and AreaType=2 and Status=1 and Coordinates is not null");
            if (!string.IsNullOrEmpty(keyword))
            {
                strSql.AppendFormat(" and (ThisName like '%{0}%' or dbo.fnGetPY(Name) like '%{1}%')", keyword, keyword.ToUpper());
            }
            int cnt = 0;
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), null, "CreateTime", "asc", currentPage, pageNum, ref cnt);
        }

        /// <summary>
        /// 获取所有马厩
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllParkings(int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select * from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from (select * from YR_ServiceArea where AreaType=2 and Status=1 and DeleteMark=1) as Parking) as Parkings where RowNumber between " + start + " and " + end + " order by CreateTime asc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        public DataTable GetVehiclesByCity(string cityname, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int pageIndex = currentPage;
            int pageSize = pageNum;
            int count = 0;
            IList<SqlParam> IList_param = new List<SqlParam>();
            strSql.Append("select * from (");
            //strSql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,LastUpdateTime FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and Longitude is not null and Latitude is not null and UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and convert(float,Electricity)>=20 and LastUpdateTime>Dateadd(n,-5, Getdate()) and dbo.GetCityName(a.CityID) like '%"+cityname+"%'");
            strSql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,LastUpdateTime FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and Longitude is not null and Latitude is not null and VehicleState=" + (int)VehicleState.Use);
            if (!string.IsNullOrEmpty(cityname))
                strSql.Append(" and dbo.GetCityName(a.CityID) like '%" + cityname + "%'");
            strSql.Append(") t");
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Name", "asc", pageIndex, pageSize, ref count);
        }

        public DataTable GetVehicles(int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int pageIndex = currentPage;
            int pageSize = pageNum;
            int count = 0;
            IList<SqlParam> IList_param = new List<SqlParam>();
            strSql.Append("select * from (");
            //strSql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,LastUpdateTime,dbo.GetCityName(a.CityID) CityName FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and Longitude is not null and Latitude is not null and UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and convert(float,Electricity)>=20 and LastUpdateTime>Dateadd(n,-5, Getdate())");
            strSql.Append("SELECT a.ID,a.Name,Brand,Model,b.Name ModelName,EngineNum,VehicleNum,VehicleGPSNum,VehicleMobile,VehicleName,LicenseNumber,VehicleColor,Displacement,FuelStyle,Transmission,PriceSystem,Mileage,UseState,VehicleState,TransportInformation,Longitude,Latitude,Address,ExpectRange,ExpectHighestSpeed,Electricity,Range,CurrentSpeed,LockState,LightState,PlatformId,c.KmPrice,c.MinutePrice,c.MinPrice,c.MaxPrice,LastUpdateTime,dbo.GetCityName(a.CityID) CityName FROM YR_Vehicles a left join YR_VehicleModel b on b.DeleteMark=1 and a.Model=b.ID left join YR_VehiclePriceRule c on c.DeleteMark=1 and a.Model=c.ModelID and a.CityID=c.CityID where a.DeleteMark=1 and Longitude is not null and Latitude is not null and VehicleState=" + (int)VehicleState.Use);
            strSql.Append(") t");
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Name", "asc", pageIndex, pageSize, ref count);
        }
    }
}
