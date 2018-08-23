using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using YR.Busines;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    public class RechargeCardManager
    {
        /// <summary>
        /// 根据ID获取参数信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Hashtable GetCardByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_RechargeCard", "ID", id);
        }

        /// <summary>
        /// 获取所有列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllCards()
        {
            return DataFactory.SqlDataBase().GetDataTable("YR_RechargeCard");
        }

        /// <summary>
        /// 充值卡列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_RechargeCard where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditCard(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_RechargeCard", "ID", _key, ht);
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelCardByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_RechargeCard", "ID", _key, ht);
        }

        /// <summary>
        /// 检测用户是否可进行充值卡充值操作
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool GetRechargeEnabled(string uid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from YR_RechargeCard where (EndDate<='" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' or Balance<=0) and UserId='").Append(uid).Append("'");
            strSql.Append(" order by RechargeTime desc");
            DataTable dt= DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt != null && dt.Rows.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取用户充值卡余额
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public decimal GetBalance(string uid)
        {
            decimal balance = 0.00m;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 balance from YR_RechargeCard where EndDate<='" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  uid='").Append(uid).Append("'");
            object value = DataFactory.SqlDataBase().GetObjectValue(strSql);
            if(value!=null && value!=DBNull.Value)
                decimal.TryParse(value.ToString(), out balance);
            return balance;
        }

        public Hashtable GetUserEnableCard(string uid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 * from YR_RechargeCard where EndDate<='" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  uid='").Append(uid).Append("'");
            return DataTableHelper.DataTableToHashtable(DataFactory.SqlDataBase().GetDataTableBySQL(strSql));
        }

    }
}
