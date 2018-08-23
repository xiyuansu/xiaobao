/*
 * 常用地址数据操作类
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
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 常用地址管理 
    /// </summary>
    public class AddressManager
    {
        /// <summary>
        /// 添加或修改常用地址信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditAddressInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_OftenAddress", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除常用地址信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelAddressInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_OftenAddress", "ID", _key, ht);
        }

        /// <summary>
        /// 常用地址列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetAddressInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_OftenAddress where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ID", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 根据id获取地址信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetAddressByID(string id)
        {
            DataSet ds = new DataSet();
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_OftenAddress where ID=@ID");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@ID";
            sqlp.FiledValue = id;
            DataTable dtaddress = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dtaddress == null || dtaddress.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dtaddress.Rows[0]);
            }
        }
        /// <summary>
        /// 通过用户获取地址列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetAddressByUser(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_OftenAddress where UserID=@UserID");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
        }
        /// <summary>
        /// 删除常用地址
        /// </summary>
        /// <param name="addressid"></param>
        /// <returns></returns>
        public int DelAddressByID(string addressid)
        {
            return DataFactory.SqlDataBase().DeleteData("YR_OftenAddress", "ID", addressid);
        }

    }
}
