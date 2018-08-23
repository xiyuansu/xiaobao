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
    /// 充电站管理 
    /// </summary>
    public class ServiceStationManager
    {
        string strTableName = "YR_ServiceStation";//").Append(strTableName).Append("
        /// <summary>
        /// 添加或修改充电站信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditServiceStation(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit(strTableName, "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除充电站信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelServiceStationByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable(strTableName, "ID", _key, ht);
        }

        /// <summary>
        /// 通过充电站ID获取充电站信息
        /// </summary>
        /// <param name="vehicleID">充电站ID</param>
        /// <returns></returns>
        public Hashtable GetServiceStationByID(string serviceStationID)
        {
            return DataFactory.SqlDataBase().GetHashtableById(strTableName, "ID", serviceStationID);
        }

        /// <summary>
        /// 充电站列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetServiceStationPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from ").Append(strTableName).Append(" where DeleteMark=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 根据经纬度分页获取某个公里范围内的充电站
        /// </summary>
        /// <param name="km">某个公里范围内</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="currentPage">当前第几页</param>
        /// <param name="pageNum">每页数据条数</param>
        /// <returns></returns>
        public DataTable GetNearServiceStation(double km, string longitude, string latitude, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,Contacts,ContactTel,Manager,ManagerTel,Address,Longitude,Latitude,VehicleTotal,VehicleUsable,HotelID,Distance from (select ROW_NUMBER() OVER (ORDER BY Distance asc ) as RowNumber,* from (select *,dbo.fnGetDistance(Longitude,Latitude,'" + longitude + "','" + latitude + "') as Distance from ").Append(strTableName).Append(" where Longitude is not null and Latitude is not null and State=1 and DeleteMark=1) as Parkings) as Parkings where Distance<=" + km + " and  RowNumber between " + start + " and " + end + " order by Distance asc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 获取所有充电站
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllServiceStations()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from ").Append(strTableName).Append(" where DeleteMark=1 order by CreateTime asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 根据充电站ID获取可用车辆列表
        /// </summary>
        /// <param name="serviceStationID"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetVehiclesByServiceStationID(string serviceStationID,int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,KMPrice,MinutesPrice,Latitude,Longitude,Address,Electricity,Range,ThumbImageUrl,RentPrice from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_vehicles.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Vehicle + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl from  yr_vehicles where UseState=" + (int)VehicleUseState.Free + " and VehicleState=" + (int)VehicleState.Use + " and DeleteMark=1 and ParkingID='" + serviceStationID + "') as Vehicle) as Vehicles where RowNumber between " + start + " and " + end + " order by CreateTime asc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 根据gps坐标获取地址信息
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public Hashtable GetProvinceCityDistrictCode(string longitude, string latitude)
        {
            try
            {
                if (!string.IsNullOrEmpty(longitude) && !string.IsNullOrEmpty(latitude))
                {
                    string province = "", city = "", district = "";
                    string address = GetLocationByGPS(Double.Parse(longitude), Double.Parse(latitude), out province, out city, out district);
                    if (address != null)
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendFormat("select dbo.GetProvinceID('{0}') ProvinceID,dbo.GetCityID('{0}','{1}') CityID,dbo.GetCountyID('{0}','{1}','{2}') CountyID", province, city, district);
                        DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
                        if (dt == null || dt.Rows.Count <= 0)
                        {
                            return null;
                        }
                        else
                        {
                            return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据经纬度坐标查询所在位置信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public string GetLocationByGPS(Double x, Double y, out string province, out string city, out string district)
        {
            province = string.Empty;
            city = string.Empty;
            district = string.Empty;
            try
            {
                string baiduMapKey = SiteHelper.GetAppsetString("BaiduMapServerKey");
                string url = "http://api.map.baidu.com/geocoder/v2/?ak=" + baiduMapKey + "&callback=renderReverse&location=" + y + "," + x + "&output=xml&pois=1&coordtype=wgs84ll ";//请求地址 
                string result = SiteHelper.GetRequest(url);

                XmlNodeList xxAddress = null;
                XmlDocument xx = new XmlDocument();
                xx.LoadXml(result);
                XmlNodeList xxList = xx.GetElementsByTagName("addressComponent");
                xxAddress = xx.GetElementsByTagName("formatted_address");

                //取得节点名为row的XmlNode集合
                foreach (XmlNode xxNode in xxList)
                {
                    XmlNodeList childList = xxNode.ChildNodes;
                    foreach (XmlNode node in xxList)
                    {
                        XmlNodeList sonNode = node.ChildNodes;
                        province = sonNode[2].InnerText;
                        city = sonNode[3].InnerText;
                        district = sonNode[4].InnerText;
                    }
                }
                return xxAddress[0].InnerText;
            }
            catch
            {
                return null;
            }
        }
    }
}
