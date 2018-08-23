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

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 车辆故障管理 
    /// </summary>
    public class VehicleFaultManager
    {
        /// <summary>
        /// 添加或修改车辆故障信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditVehicleFaultInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleFault", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除车辆故障信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleFaultInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehicleFault", "ID", _key, ht);
        }

        /// <summary>
        /// 通过故障ID获取车辆故障信息
        /// </summary>
        /// <param name="vehicleID">车辆ID</param>
        /// <returns></returns>
        public DataTable GetVehicleFaultInfoByID(string faultID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from YR_VehicleFault where ID='" + faultID + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb);
        }

        /// <summary>
        /// 车辆故障列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetVehicleFaultInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_VehicleFault where isnull(DeleteMark,0)!=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }
    }
}
