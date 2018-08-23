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
    /// 系统字典管理 
    /// </summary>
    public class DictManager
    {

        /// <summary>
        /// 添加或修改
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditMessageInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("Base_DictList", "ID", _key, ht);
        }
        /// <summary>
        /// 列表，分页
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
            strSql.Append(@"select * from Base_DictList where DeleteMark=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Sort", "Asc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取字典条目列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDict()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from Base_Dict ");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }


        /// <summary>
        /// 获取字典条目列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDictList(string dictid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select id,name,remark from Base_DictList where deletemark=1 and DictID='{0}' order by Sort asc", dictid);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }


        public string GetItemName(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select id,name,remark from Base_DictList where ID='{0}'", id);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["name"].ToString();
            else
                return "";
        }

        public string GetItemID(string name,string dictId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from Base_DictList where DictID='{0}' and Name='{1}'", dictId,name);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["id"].ToString();
            else
                return "";
        }


        public DataTable GetItem(string dicId, string basDicName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select ID,Name,Remark,Sort,DictID,DeleteMark,Longitude,Latitude from Base_DictList where Name='{0}' and DictID='{1}'", basDicName, dicId);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 根据id获取字典详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetDicItemByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT ID,Name,Remark,Sort,DictID,CityCode,ZipCode,Longitude,Latitude,DeleteMark FROM Base_DictList where deletemark=1 and id='{0}'", id);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return new Hashtable();
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }
    }
}
