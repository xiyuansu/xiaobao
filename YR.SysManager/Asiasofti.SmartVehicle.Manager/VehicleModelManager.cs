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
using System.Xml;


namespace Asiasofti.SmartVehicle.Manager
{
    public class VehicleModelManager
    {
        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleModel", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelVehicleModelID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_VehicleModel", "ID", _key, ht);
        }

        /// <summary>
        /// 通过ID获取信息
        /// </summary>
        /// <param name="vehicleID">车型ID</param>
        /// <returns></returns>
        public Hashtable GetModelByID(string modelID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_VehicleModel", "ID", modelID);
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
        public DataTable GetPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select s.ID,s.Name,s.Intro,
                            '<a href=javascript:ReviewInfo(""'+s.Images+'""); >浏览图片</a>' Images,
                            s.CreateTime,s.UpdateTime,Operator,Sort 
                            from YR_VehicleModel s where DeleteMark=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "Sort", "Asc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取所有车型信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllModelInfo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_VehicleModel where DeleteMark!=0 order by sort");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

    }
}
