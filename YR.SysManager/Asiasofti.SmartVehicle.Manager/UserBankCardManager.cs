using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YR.Common;
using YR.Busines;
using System.Data;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;


namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户银行卡管理
    /// </summary>
    public class UserBankCardManager
    {
        /// <summary>
        /// 添加或修改用户银行卡
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserBankCardInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserBankCard", "ID", _key, ht);
        }
        /// <summary>
        /// 通过主键ID删除银行卡
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelUserBankCardInfoByID(string _key)
        {
            return DataFactory.SqlDataBase().DeleteData("YR_UserBankCard", "ID", _key);
        }
        /// <summary>
        /// 通过用户得到银行卡信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUserBankCard(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from yr_userbankcard where UserID=@UserID");
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
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }
    }
}
