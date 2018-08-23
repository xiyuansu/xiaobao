/*
 * 维修站数据操作类
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
    /// 维修站管理 
    /// </summary>
    public class PitsManager
    {
        /// <summary>
        /// 添加或修改车辆信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddPitsInfo(Hashtable ht, string _key, string[] userIDs)
        {
            // 添加维修站
            DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Pits", "ID", _key, ht);
            if (userIDs != null)
            {
                foreach (string uID in userIDs)
                {
                    if (uID.Length > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" Insert into YR_pitsUser(ID,PitsID,UserID,CreateTime)");
                        sb.Append(" Values(@ID,@PitsID,@UserID,getdate())");
                        SqlParam[] parmAdd = new SqlParam[] { 
                                     new SqlParam("@ID",CommonHelper.GetGuid),
                                     new SqlParam("@PitsID", ht["ID"]),
                                     new SqlParam("@UserID", uID)};
                        DataFactory.SqlDataBase().ExecuteBySql(sb, parmAdd);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 修改车辆信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool EditPitsInfo(Hashtable ht, string _key, string[] userIDs)
        {
            // 修改维修站
            DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Pits", "ID", _key, ht);
            StringBuilder sb = new StringBuilder();
            // 先删除维修站人员后添加
            sb.Append(" delete from YR_pitsUser where PitsID='" + _key + "'");
            DataFactory.SqlDataBase().ExecuteBySql(sb);
            if (userIDs != null)
            {
                foreach (string uID in userIDs)
                {
                    if (uID.Length > 0)
                    {
                        sb = new StringBuilder();
                        sb.Append(" Insert into YR_pitsUser(ID,PitsID,UserID,CreateTime)");
                        sb.Append(" Values(@ID,@PitsID,@UserID,getdate())");
                        SqlParam[] parmAdd = new SqlParam[] { 
                                     new SqlParam("@ID",CommonHelper.GetGuid),
                                     new SqlParam("@PitsID", ht["ID"]),
                                     new SqlParam("@UserID", uID)};
                        DataFactory.SqlDataBase().ExecuteBySql(sb, parmAdd);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 通过主键ID删除维修站信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelPitsInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_Pits", "ID", _key, ht);
        }

        /// <summary>
        /// 通过维修站ID获取维修站信息
        /// </summary>
        /// <param name="pitsID">维修站ID</param>
        /// <returns></returns>
        public Hashtable GetPitsInfoByID(string pitsID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_Pits", "ID", pitsID);
        }

        /// <summary>
        /// 通过维修站ID获取维修站信息
        /// </summary>
        /// <param name="vehicleID">维修站ID</param>
        /// <returns></returns>
        public Hashtable GetPitsInfoByID(string chargingPiesID, string weidu, string jingdu)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ID,Name,Address,Telephone,Mobile,Longitude,Latitude, ID as Description,dbo.fnGetDistance(Latitude,Longitude,'" + weidu + "','" + jingdu + "') as Distance,ServiceTime from YR_Pits where 1=1 ");
            return DataFactory.SqlDataBase().GetHashtableBySQL(sql, "ID", chargingPiesID);
        }

        /// <summary>
        /// 维修站列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetPitsInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.[User_Name] from YR_Pits a LEFT JOIN Base_UserInfo b ON a.OwnerID=b.[User_ID] where 1=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 维修站列表，分页
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPitsInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.[User_Name] from YR_Pits a LEFT JOIN Base_UserInfo b ON a.OwnerID=b.[User_ID]");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 根据经纬度分页获取某个公里范围内的可用的维修站
        /// </summary>
        /// <param name="km">某个公里范围内</param>
        /// <param name="weidu">纬度</param>
        /// <param name="jingdu">经度</param>
        /// <param name="currentPage">当前第几页</param>
        /// <param name="pageNum">每页数据条数</param>
        /// <returns></returns>
        public DataTable GetNearPits(double km, string weidu, string jingdu, int currentPage, int pageNum)
        {
            StringBuilder strSql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strSql.Append("select ID,Name,Address,Telephone,Mobile,Longitude,Latitude,ServiceTime,ID AS Description,ThumbImageUrl,Distance from (select ROW_NUMBER() OVER (ORDER BY Distance asc ) as RowNumber,* from (select *,(SELECT TOP 1 YR_SystemImages.ImageUrl From YR_SystemImages  WHERE YR_SystemImages.RelationID=yr_pits.ID AND YR_SystemImages.Category=" + (int)ImagesCategory.Pits + " AND YR_SystemImages.[Type]=" + (int)ImagesType.Thumbnail + ") AS ThumbImageUrl,dbo.fnGetDistance(Latitude,Longitude,'" + weidu + "','" + jingdu + "') as Distance from  yr_pits where Longitude is not null and Latitude is not null and State=" + (int)PitsState.Enable + ")  as Pit) as Pits where Distance<=" + km + " and  RowNumber between " + start + " and " + end + "");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        #region 维修站员工

        /// <summary>
        /// 通过维修站ID获取维修站维修人员信息
        /// </summary>
        /// <param name="pitsID">维修站ID</param>
        /// <returns></returns>
        public DataTable GetPitsUserInfoByID(string pitsID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,PitsID,UserID,CreateTime from YR_PitsUser where PitsID='" + pitsID + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        #endregion
    }
}
