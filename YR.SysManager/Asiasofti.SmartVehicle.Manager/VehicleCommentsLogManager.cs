/*
 * 车辆评论记录数据操作类
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
    /// 车辆评论记录管理 
    /// </summary>
    public class VehicleCommentsLogManager
    {
        /// <summary>
        /// 添加或修改车辆信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditVehicleCommentsLogInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleCommentsLog", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除车辆评论记录(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleCommentsLogInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehicleCommentsLog", "ID", _key, ht);
        }

        /// <summary>
        /// 车辆评论记录列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetVehicleCommentsLogInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.VehicleName,b.LicenseNumber,b.VehicleGPSNum,c.BindPhone,c.RealName from YR_VehicleCommentsLog a left join YR_Vehicles b on a.VehicleID=b.ID left join YR_UserInfo c on a.UserID=c.ID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CommentsTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 分页得到车辆评论
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetVehicleCommentsByVehicle(string vid, int currentPage, int pageNum)
        {
            StringBuilder strsql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strsql.Append(@"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CommentsTime desc ) as RowNum,UserNickName,CommentsContent,CommentsTime FROM dbo.YR_VehicleCommentsLog WHERE VehicleID=@VehicleID  and  State=").Append(VehicleCommentsState.CheckedSuccess.GetHashCode().ToString()).Append(" ) as VehicleComments where RowNum between  ").Append(start).Append(" and  ").Append(end);
            SqlParam param = new SqlParam();
            param.FieldName = "@VehicleID";
            param.FiledValue = vid;
            return DataFactory.SqlDataBase().GetDataTableBySQL(strsql, new SqlParam[] { param });
        }

    }
}
