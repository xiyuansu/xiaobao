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

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 系统配置参数管理 
    /// </summary>
    public class SysSettingManager
    {
        /// <summary>
        /// 添加或修改
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("Sys_Setting", "ID", _key, ht);
        }

        /// <summary>
        /// 获取帮助列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetBaseDictList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select * from [dbo].[Base_DictList] where DictID='01'");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 根据id获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetItemByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT * FROM Sys_Setting where  ID='{0}'", id);
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

        /// <summary>
        /// 文章列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetDataInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select s.ID,s.SettingKey,
                            s.Name,
                            CASE WHEN CHARINDEX('image', LOWER(SettingKey)) > 0 THEN '<a href=javascript:ReviewInfo(""'+s.SettingContent+'""); >浏览图片</a>' else s.SettingContent
                            END as SettingContent,
                            s.Description,s.SettingType,bd.Name dicName
                            from [dbo].[Sys_Setting] s
                            left join [Base_DictList] bd on s.SettingType=bd.ID where 1=1 ");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ID", "Desc", pageIndex, pageSize, ref count);
        }
       
        /// <summary>
        /// 获取所有配置参数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataTable GetSettings(string type)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select SettingKey,Name,SettingContent,Description from Sys_Setting");
            string where = "";
            if(!string.IsNullOrEmpty(type))
            {
                if (where.EndsWith(" and "))
                    where += " and ";
                where += string.Format("SettingType='{0}'",type);
            }
            if (where.Length > 0)
                sql.Append(" where ").Append(where);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        /// <summary>
        /// 根据ID获取参数信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Hashtable GetSettingByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("Sys_Setting", "ID", id);
        }

        public DataTable GetSettingByKey(string key)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select SettingKey,Name,SettingContent,Description from Sys_Setting where SettingKey='").Append(key).Append("'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        /// <summary>
        /// 根据参数名获取参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByKey(string key)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select SettingContent from Sys_Setting where SettingKey='").Append(key).Append("'");
            return DataFactory.SqlDataBase().GetSingleValueBySQL(sql);
        }

        /// <summary>
        /// 获取所有列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllSetting()
        {
            return DataFactory.SqlDataBase().GetDataTable("Sys_Setting");
        }
        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditSetting(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("Sys_Setting", "ID", _key, ht);
        }
    }
}
