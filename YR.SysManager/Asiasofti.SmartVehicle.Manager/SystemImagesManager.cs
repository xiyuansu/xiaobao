using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.Busines;
using YR.DataBase;

namespace Asiasofti.SmartVehicle.Manager
{
    public class SystemImagesManager
    {
        /// <summary>
        /// 添加图片信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditImageInfo(Hashtable ht, string fileID)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_SystemImages", "ID", fileID, ht);
        }

        /// <summary>
        /// 删除图片信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelImageInfoByID(string fileID)
        {
            return DataFactory.SqlDataBase().DeleteData("YR_SystemImages", "ID", fileID);
        }

        /// <summary>
        /// 根据车辆id获取该车辆关联的图片信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public DataTable GetImagesByVehicleID(string vehicleID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from YR_SystemImages where RelationID ='").Append(vehicleID).Append("' and Category =").Append((int)ImagesCategory.Vehicle);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        /// <summary>
        /// 根据维修站id获取该维修站关联的图片信息
        /// </summary>
        /// <param name="pitsID"></param>
        /// <returns></returns>
        public DataTable GetImagesByPitsID(string pitsID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from YR_SystemImages where RelationID ='").Append(pitsID).Append("' and Category =").Append((int)ImagesCategory.Pits);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        /// <summary>
        /// 根据充电桩id获取该充电桩关联的图片信息
        /// </summary>
        /// <param name="chargingPieID"></param>
        /// <returns></returns>
        public DataTable GetImagesByChargingPieID(string chargingPieID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from YR_SystemImages where RelationID ='").Append(chargingPieID).Append("' and Category =").Append((int)ImagesCategory.ChargingPie);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }
    }
}
