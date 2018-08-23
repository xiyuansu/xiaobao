/*
 * 车辆保养记录数据操作类
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

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 车辆保养记录管理 
    /// </summary>
    public class VehicleMaintainLogManager
    {
        /// <summary>
        /// 添加或修改车辆信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditVehicleMaintainLogInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleMaintainLog", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除车辆保养记录(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleMaintainLogInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehicleMaintainLog", "ID", _key, ht);
        }

        /// <summary>
        /// 通过ID获取保养记录信息
        /// </summary>
        /// <param name="vehicleMaintainLogID">保养记录ID</param>
        /// <returns></returns>
        public Hashtable GetVehicleMaintainLogInfoByID(string vehicleMaintainLogID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_VehicleMaintainLog", "ID", vehicleMaintainLogID);
        }

        /// <summary>
        /// 车辆保养记录列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetVehicleMaintainLogInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.Name AS PitsName,c.Name AS VehicleName from YR_VehicleMaintainLog a ");
            strSql.Append("INNER JOIN YR_Pits b ON a.PitID=b.ID INNER JOIN YR_Vehicles c ON a.VehicleID=c.ID ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 根据车辆分页获取车辆维修记录 按维修时间倒序
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetVehicleMaintainLogByVehicleID(string vid, int currentPage, int pageNum)
        {
            StringBuilder strsql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strsql.Append(@"select * from (select ROW_NUMBER() OVER (ORDER BY MaintainTime desc ) as RowNum,ID,MaintainType,MaintainItems,Remark,MaintainTime from dbo.YR_VehicleMaintainLog where VehicleID=@VehicleID) VehicleMaintainLog where RowNum between ").Append(start).Append(" and ").Append(end);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@VehicleID";
            sqlp.FiledValue = vid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strsql, new SqlParam[] { sqlp });
            return dt;
        }
    }
}
