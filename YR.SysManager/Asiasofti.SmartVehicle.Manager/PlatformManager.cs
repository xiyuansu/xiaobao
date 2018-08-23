/*
 * 平台数据操作类
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

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 平台管理 
    /// </summary>
    public class PlatformManager
    {
        /// <summary>
        /// 添加或修改平台信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditPlatformInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehiclePlatform", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除平台信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelplatformInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehiclePlatform", "ID", _key, ht);
        }

        /// <summary>
        /// 通过平台ID获取平台信息
        /// </summary>
        /// <param name="vehicleID">平台ID</param>
        /// <returns></returns>
        public Hashtable GetPlatformInfoByID(string vehicleID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_VehiclePlatform", "ID", vehicleID);
        }
        /// <summary>
        /// 平台列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetPlatformInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_VehiclePlatform where DeleteMark!=0 ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Sort", "Asc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 获取所有平台信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPlatformInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_VehiclePlatform where DeleteMark!=0 order by Sort asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }
    }
}
