/*
 * 众筹财务信息数据操作类
 * 作者：SJ
 * 时间：2015-05-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YR.Busines;
using System.Collections;
using System.Data;
using YR.Common.DotNetCode;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 众筹财务信息管理
    /// </summary>
    public class UserRaiseFinancialManager
    {
        /// <summary>
        /// 添加或修改众筹财务信息（交易记录）
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserRaiseFinancialInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserRaiseFinancial", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除众筹财务信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelUserRaiseFinancialInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_UserRaiseFinancial", "ID", _key, ht);
        }

        /// <summary>
        /// 众筹财务信息列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserRaiseFinancialInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.BindPhone,b.RealName,c.OperatorName,d.OrderNum from YR_UserRaiseFinancial a left join YR_UserInfo b ");
            strSql.Append("on a.UserID=b.ID left join (SELECT ID,NickName AS OperatorName FROM YR_UserInfo ");
            strSql.Append(" UNION SELECT [User_ID],[USER_NAME] AS OperatorName FROM Base_UserInfo) c on a.Operator=c.ID left join YR_Orders d on a.OrderID=d.ID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ChangesTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 会员众筹财务流水记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserRaiseFinancialInfoByUser(string uid, int currentPage, int pageNum)
        {
            StringBuilder sql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            sql.Append(@"select * from (select ROW_NUMBER() OVER (ORDER BY ChangesTime desc ) as RowNum,ID,UserID,ChangesCoin,ChangesType,CurrentCoin,Remark,ChangesTime,ReflectWay from dbo.YR_UserRaiseFinancial where userid=@UserID and state=").Append(UserFinancialState.Effect.GetHashCode().ToString()).Append(") as URF where  RowNum between ").Append(start.ToString()).Append(" and ").Append(end.ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }
    }
}
