using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using YR.Common;
using YR.Busines;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using Asiasofti.SmartVehicle.Common;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户众筹提现管理
    /// </summary>
    public class UserRaiseWithdrawalManager
    {
        /// <summary>
        /// 添加或修改用户众筹提现申请
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserRaiseWithdrawalInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserRaiseWithdrawal", "ID", _key, ht);
        }

        /// <summary>
        /// 处理提现申请
        /// </summary>
        /// <returns></returns>
        public int UpdateUserRaiseWithdrawalState(string id, string applyState,string remark)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update YR_UserRaiseWithdrawal set ApplyState=" + applyState + ",remark='" + remark + "',DoTime='" + SiteHelper.GetWebServerCurrentTime() + "' where ID='" + id + "'");
            return DataFactory.SqlDataBase().ExecuteBySql(sb);
        }
        /// <summary>
        /// 通过主键ID删除用户众筹提现申请
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelUserRaiseWithdrawalInfoByID(string _key)
        {
            return DataFactory.SqlDataBase().DeleteData("YR_UserRaiseWithdrawal", "ID", _key);
        }

        /// <summary>
        /// 通过id获取提现申请记录信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUserRaiseWithdrawalById(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from YR_UserRaiseWithdrawal where ID=@ID");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@ID";
            sqlp.FiledValue = id;
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
        /// 通过用户得到该用户所有的提现记录按时间倒序
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserRaiseWithdrawalByUser(string uid, int currentPage, int pageNum)
        {
            StringBuilder sql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            sql.Append("select * from (select ROW_NUMBER() OVER (ORDER BY ApplyTime desc ) as RowNum, * from YR_UserRaiseWithdrawal where UserID=@UserID ) as UserRaiseWihtdrawal where RowNum between ").Append(start).Append(" and ").Append(end);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return dt;
            }
        }

        /// <summary>
        /// 获取提现申请列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserRaiseWithdrawalInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" select a.*,b.BindPhone,b.RealName,c.AccountName,c.Account,c.BankName,c.OpenBank from YR_UserRaiseWithdrawal a left join YR_UserInfo b ");
            strSql.Append(" on a.UserID=b.ID LEFT JOIN YR_UserBankCard c ON b.ID=c.UserID where 1=1 ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ApplyState,ApplyTime Desc", "", pageIndex, pageSize, ref count);
        }
    }
}
