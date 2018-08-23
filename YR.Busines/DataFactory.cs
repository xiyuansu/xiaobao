using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YR.DataBase;
using PDA_Service.DataBase.DataBase.SqlServer;
using YR.Common.DotNetConfig;

namespace YR.Busines
{
    /// <summary>
    /// 连接数据库服务工厂
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// 链接 SqlServer 数据库
        /// </summary>
        /// <returns></returns>
        public static IDbHelper SqlDataBase()
        {
            return new SqlServerHelper(ConfigHelper.GetAppSettings("SqlServer_YR_DB"));
        }

        public static IDbHelper SqlDataBase(string connString)
        {
            return new SqlServerHelper(connString);
        }
    }
}
