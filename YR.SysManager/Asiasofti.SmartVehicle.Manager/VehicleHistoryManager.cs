/*
 * 会员行车历史数据操作类
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
    /// 行车历史管理 
    /// </summary>
    public class VehicleHistoryManager
    {
        /// <summary>
        /// 添加或修改行车历史信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditVehicleHistoryInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleDriveLog", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除行车历史信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleHistoryInfoByID(Hashtable ht,string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehicleDriveLog", "ID", _key, ht);
        }

        /// <summary>
        /// 行车历史列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetVehicleHistoryInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.ID,a.GPSNum,a.Longitude,a.Latitude,a.Speed,a.Power,a.RecordTime,a.DriveStyle,a.OprResult,a.ReturnResult,c.Name,c.LicenseNumber from YR_VehicleDriveLog a left join YR_Vehicles c on a.GPSNum=c.VehicleGPSNum where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "RecordTime", "Desc", pageIndex, pageSize, ref count);
        }

    }
}
