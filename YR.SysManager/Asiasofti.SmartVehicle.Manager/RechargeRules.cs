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
    public class RechargeRules
    {
        /// <summary>
        /// 根据ID获取参数信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Hashtable GetByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_RechargeRules", "ID", id);
        }

        /// <summary>
        /// 获取所有列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAll()
        {
            return DataFactory.SqlDataBase().GetDataTable("YR_RechargeRules");
        }

        /// <summary>
        /// 充电桩列表，分页
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
            strSql.Append(@"select * from YR_RechargeRules where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Sort", "Asc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditSetting(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_RechargeRules", "ID", _key, ht);
        }

        /// <summary>
        /// 获取充值活动列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAvailableRules(decimal? money)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            now += " 00:00:00";
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ID,Name,Presented,PresentedFlag,ChargeMoney,PresentMoney,BeginTime,EndTime,CreateTime FROM YR_RechargeRules");
            strSql.Append(" where ChargeMoney is not null and (BeginTime<='" + now + "' and EndTime>='" + now + "')");
            if (money.HasValue)
                strSql.Append(" and PresentMoney>0 and ChargeMoney=" + money.Value);
            strSql.Append(" order by Sort asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt == null || dt.Rows.Count == 0)
            {
                strSql.Clear();
                strSql.Append("SELECT ID,Name,Presented,PresentedFlag,ChargeMoney,PresentMoney,BeginTime,EndTime,CreateTime FROM YR_RechargeRules");
                strSql.Append(" where (BeginTime<='" + now + "' and EndTime>='" + now + "') and chargemoney is null");
                strSql.Append(" order by CreateTime desc");
                dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            }
            return dt;
        }
    }
}
