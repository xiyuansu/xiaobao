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
using YR.Common.DotNetBean;
using System.Configuration;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 充电桩管理 
    /// </summary>
    public class ChargingPointsManager
    {
        /// <summary>
        /// 添加或修改充电桩信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditChargingPoint(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_ChargingPoints", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除充电桩信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelChargingPointByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_ChargingPoints", "ID", _key, ht);
        }

        /// <summary>
        /// 通过充电桩ID获取充电桩信息
        /// </summary>
        /// <param name="id">充电桩ID</param>
        /// <returns></returns>
        public Hashtable GetChargingPointByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_ChargingPoints", "ID", id);
        }

        /// <summary>
        /// 通过充电桩ID获取充电桩信息
        /// </summary>
        /// <param name="id">充电桩ID</param>
        /// <returns></returns>
        public Hashtable GetChargingPointByID(string id,string longitude ,string latitude)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ID,Name,IPAddr,IMEI,Longitude,Latitude,Address,State,UseState,StationID,DeleteMark,CreateTime,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance FROM YR_ChargingPoints where 1=1 ");
            return DataFactory.SqlDataBase().GetHashtableBySQL(sql, "ID", id);
        }

        /// <summary>
        /// 充电桩列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetChargingPointPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT ID,Name,IPAddr,IMEI,Longitude,Latitude,Address,State,UseState,Phone,StationID,DeleteMark,CreateTime,LastUpdateTime,isnull(datediff(n,LastUpdateTime,Getdate()),9999) diffminutes FROM YR_ChargingPoints where DeleteMark=1 ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 根据经纬度分页获取某个公里范围内的可用的充电桩
        /// </summary>
        /// <param name="km">某个公里范围内</param>
        /// <param name="weidu">纬度</param>
        /// <param name="jingdu">经度</param>
        /// <param name="currentPage">当前第几页</param>
        /// <param name="pageNum">每页数据条数</param>
        /// <returns></returns>
        public DataTable GetNearChargingPoints(double km, string longitude, string latitude, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,IPAddr,IMEI,Longitude,Latitude,Address,State,UseState,StationID,DeleteMark,CreateTime,Distance from (select ROW_NUMBER() OVER (ORDER BY Distance asc ) as RowNumber,* from (select *,dbo.fnGetDistance(Latitude,Longitude,'" + latitude + "','" + longitude + "') as Distance from YR_ChargingPoints where Longitude is not null and Latitude is not null and State=1 and UseState=1) as YR_ChargingPoints) as YR_ChargingPoints where Distance<=" + km + " and  RowNumber between " + start + " and " + end + "");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }


        public Hashtable GetBizIPPortByGPSID(string imei)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT BizSocketIP IP,BizSocketPort PORT FROM YR_ChargingPoints where IMEI=@IMEI");
            SqlParam param = new SqlParam();
            param.FieldName = "@IMEI";
            param.FiledValue = imei;
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
        /// 远程控制充电桩开锁关锁
        /// </summary>
        /// <returns></returns>
        public bool GPSRemoteControlLock(string gpsID, bool isLock)
        {
            try
            {
                bool returnTrue = false;
                byte[] sendcmd = new byte[13];
                //数据包类型两位
                sendcmd[0] = 0x68;
                sendcmd[1] = 0x68;
                //命令类型一位
                if (!isLock)
                {
                    //开
                    sendcmd[2] = 0x01;
                }
                else
                {
                    //关
                    sendcmd[2] = 0x02;
                }
                //数据包长度,数据包长度2位+IMIE号8位
                int datalen = 10;
                string strlen = datalen.ToString("X4");
                TcpClient client = new TcpClient();
                byte[] bytelen = client.strToToHexByte(strlen);
                byte[] byteimie = client.StringToBytes(gpsID);
                Array.Copy(bytelen, 0, sendcmd, 3, 2);
                Array.Copy(byteimie, 0, sendcmd, 5, 8);
                string serverIp = SiteHelper.GetAppsetString("server_ip");
                string serverPort =SiteHelper.GetAppsetString("server_port");
                Hashtable ht = GetBizIPPortByGPSID(gpsID);
                if (ht != null && ht.Keys.Count > 0)
                {
                    if (!string.IsNullOrEmpty(ht["IP"].ToString()))
                        serverIp = ht["IP"].ToString();
                    if (!string.IsNullOrEmpty(ht["PORT"].ToString()))
                        serverPort = ht["PORT"].ToString();
                }
                bool result = client.Connect(serverIp, int.Parse(serverPort));
                if (result)
                {
                    client.Send(sendcmd);
                    string response = client.ReceiveString();
                    returnTrue = response.Contains("ok") ? true : false;
                }
                return returnTrue;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取充电站列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetChargingStations(string name, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,Contacts,ContactTel,Manager,ManagerTel,Address,Longitude,Latitude,CustomerID,CreateTime from (select ROW_NUMBER() OVER (ORDER BY CreateTime asc ) as RowNumber,* from YR_ServiceStation where DeleteMark=1 and State=1) as Stations where RowNumber between " + start + " and " + end);
            if (!string.IsNullOrEmpty(name))
                strSql.Append(" and Name like '%" + name + "%'");
            strSql.Append(" order by CreateTime desc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

    }
}
