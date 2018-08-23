/*
 * 会员财务信息数据操作类
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
using YR.Common.DotNetData;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.DataBase;

namespace Asiasofti.SmartVehicle.Manager
{
    public class UserFinancialManager
    {
        /// <summary>
        /// 添加或修改租车会员财务信息（交易记录）
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserFinancialInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserFinancial", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除租车会员财务信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelUserFinancialInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_UserFinancial", "ID", _key, ht);
        }

        /// <summary>
        /// 租车会员财务信息列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserFinancialInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.NickName,b.RealName,b.BindPhone,c.OperatorName from YR_UserFinancial a left join YR_UserInfo b ");
            strSql.Append("on a.UserID=b.ID left join (SELECT ID,NickName AS OperatorName FROM YR_UserInfo ");
            strSql.Append(" UNION SELECT [User_ID],[USER_NAME] AS OperatorName FROM Base_UserInfo) c on a.Operator=c.ID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ChangesTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 通过id得到用户待支付的充值记录
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Hashtable GetUserFinancialPayInfoByID(string oid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select ID,RTRIM(Remark)+RTRIM(ChangesAmount)+'元' as subject,OrderNum as out_trade_no, ChangesAmount as total_fee from YR_UserFinancial where ID=@ID and State=").Append((int)UserFinancialState.NewSubmit);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@ID";
            sqlp.FiledValue = oid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
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
        /// 通过支付单号得到用户待支付的充值记录
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Hashtable GetUserFinancialPayInfoByPayID(string orderpayid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select ID,RTRIM(Remark)+RTRIM(ChangesAmount)+'元' as subject,OrderNum as out_trade_no, ChangesAmount as total_fee,OrderPayID,UserID from YR_UserFinancial where OrderPayID=@OrderPayID and State=").Append((int)UserFinancialState.NewSubmit);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@OrderPayID";
            sqlp.FiledValue = orderpayid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
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
        /// 根据充值订单号获取用户提交的充值信息
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public DataTable GetUserFinancialInfoByOrderNum(string ordernum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_UserFinancial where OrderNum=@OrderNum and State=").Append(UserFinancialState.NewSubmit.GetHashCode()).Append(" order by ChangesTime desc");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@OrderNum";
            sqlp.FiledValue = ordernum;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }
        /// <summary>
        /// 根据支付编号获取用户提交的充值信息
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public DataTable GetUserFinancialInfoByPayId(string orderpayid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_UserFinancial where OrderPayID=@OrderPayID and State=").Append(UserFinancialState.NewSubmit.GetHashCode()).Append(" order by ChangesTime desc");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@OrderPayID";
            sqlp.FiledValue = orderpayid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }
        /// <summary>
        /// 根据会员获取会员的财务流水记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserFinancialInfoByUser(string uid, int currentPage, int pageNum)
        {
            StringBuilder sql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            sql.Append(@"select * from (select ROW_NUMBER() OVER (ORDER BY ChangesTime desc ) as RowNum, ID,UserID,ChangesAmount,ChangesType,CurrentBalance,Remark,ChangesTime,OperatorWay from YR_UserFinancial where UserID=@UserID and State=").Append(UserFinancialState.Effect.GetHashCode()).Append(") as UF  where  RowNum between ").Append(start.ToString()).Append(" and ").Append(end.ToString()); ;
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }

        /// <summary>
        /// 根据用户id,支付状态，交易类型获取最近一次交易信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="state"></param>
        /// <param name="changes_type"></param>
        /// <returns></returns>
        public Hashtable GetLatestUserFinancialInfo(string uid,UserFinancialState state,UserFinancialChangesType changes_type)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select ID,UserID,OrderNum,ChangesTime,ChangesAmount,ChangesType,CurrentBalance,Remark,OperatorWay,OrderPayID from YR_UserFinancial where State=@State and ChangesType=@ChangesType and UserID=@UserID");
            SqlParam[] sqlparams = new SqlParam[] { 
                new SqlParam("@UserID",uid),
                new SqlParam("@State",state.GetHashCode()),
                new SqlParam("@ChangesType",changes_type.GetHashCode())
            };
            sql.Append(" order by ChangesTime desc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, sqlparams);
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
        /// 根据支付状态，交易类型获取交易信息列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="state"></param>
        /// <param name="changes_type"></param>
        /// <returns></returns>
        public DataTable GetUserFinancialList(string uid, DateTime? begintime, DateTime? endtime, UserFinancialState state, UserFinancialChangesType? changes_type)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select ID,UserID,OrderNum,ChangesAmount,ChangesType,CurrentBalance,Remark,ChangesTime,OperatorWay,OrderPayID from YR_UserFinancial where State=@State");
            if (!string.IsNullOrEmpty(uid))
                sql.Append(" and UserID='" + uid + "'");
            if (endtime.HasValue)
                sql.Append(" and ChangesTime>='" + begintime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (begintime.HasValue)
                sql.Append(" and ChangesTime<='" + endtime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (changes_type.HasValue)
                sql.Append(" and ChangesType=" + changes_type.Value.GetHashCode());
            SqlParam[] sqlparams = new SqlParam[] { 
                new SqlParam("@State",state.GetHashCode())
            };
            sql.Append(" order by ChangesTime asc");

            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, sqlparams);
            return dt;
        }

        /// <summary>
        /// 判断用户是否是首次充值
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsUserFirstCharge(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_UserFinancial where UserID=@UserID and State=").Append((int)UserFinancialState.Effect);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
