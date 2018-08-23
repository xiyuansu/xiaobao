using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using YR.Busines;
using YR.DataBase;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 文章管理
    /// </summary>
    public class ArticleManager
    {
        /// <summary>
        /// 根据文章标识获取文章信息
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        public Hashtable GetArticleUrlByArticleMark(string mark)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top 1 * from YR_Articles where ArticleMark='").Append(mark.Trim()).Append("'");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
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
        /// 添加或修改文章信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditArticleInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Articles", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除文章信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelArticleInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_Articles", "ID", _key, ht);
        }

        /// <summary>
        /// 通过维修站ID获取文章信息
        /// </summary>
        /// <param name="articleID">文章ID</param>
        /// <returns></returns>
        public Hashtable GetArticleInfoByID(string articleID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_Articles", "ID", articleID);
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
        public DataTable GetDataPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_Articles where DeleteMark=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Sort", "Asc", pageIndex, pageSize, ref count);
        }
    }
}
