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
    /// 客户管理 
    /// </summary>
    public class CustomerManager
    {
        string strTableName = "YR_Customer";
        /// <summary>
        /// 添加或修改客户信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditCustomer(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit(strTableName, "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除酒店信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelCustomerByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable(strTableName, "ID", _key, ht);
        }

        /// <summary>
        /// 通过酒店ID获取酒店信息
        /// </summary>
        /// <param name="vehicleID">酒店ID</param>
        /// <returns></returns>
        public Hashtable GetCustomerByID(string parkingID)
        {
            return DataFactory.SqlDataBase().GetHashtableById(strTableName, "ID", parkingID);
        }

        /// <summary>
        /// 酒店列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetCustomerPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(@"select * from ").Append(strTableName).Append(" where DeleteMark=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取所有客户
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllCustomers()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(@"select * from ").Append(strTableName).Append(" where DeleteMark=1 order by CreateTime asc");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }
    }
}
