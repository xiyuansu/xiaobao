using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YR.Busines;
using System.Collections;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 车辆报警管理 
    /// </summary>
    public class VehicleAlarmManager
    {
        /// <summary>
        /// 添加或编辑数据
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_VehicleAlarms", "ID", _key, ht);
        }

        /// <summary>
        /// 报警信息列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetList(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT YR_VehicleAlarms.ID,VehicleID,YR_Vehicles.Name VehicleName,YR_Vehicles.LicenseNumber,IMEI,AlarmType,Speed,AlarmTime,AlarmStatus,OperatorID,OperatorType,OperateRemark,OperateTime,YR_VehicleAlarms.CreateTime FROM YR_VehicleAlarms left join YR_Vehicles on YR_VehicleAlarms.VehicleID=YR_Vehicles.ID WHERE 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 查询30分钟内的超速记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetOverSpeedList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT VehicleID,count(VehicleID) AlarmCount FROM YR_VehicleAlarms where AlarmType = 6 and isnull(datediff(n, CreateTime, Getdate()),9999)<=30 group by VehicleID");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 查询1小时内的越界记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutAreaList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT VehicleID,count(VehicleID) AlarmCount FROM YR_VehicleAlarms where AlarmType = 4 and isnull(datediff(n, CreateTime, Getdate()),9999)<=60 group by VehicleID");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 获取数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetInfoByID(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT YR_VehicleAlarms.ID,VehicleID,YR_Vehicles.Name VehicleName,AlarmType,AlarmTime,AlarmStatus,OperatorID,OperatorType,OperateRemark,OperateTime,YR_VehicleAlarms.CreateTime FROM YR_VehicleAlarms left join YR_Vehicles on YR_VehicleAlarms.VehicleID=YR_Vehicles.ID WHERE YR_VehicleAlarms.id='{0}'",id);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return new Hashtable();
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
            
        }

        public string GetOperator(string operator_id,string operator_type)
        {
            StringBuilder strSql = new StringBuilder();
            if(operator_type=="1")
            {
                strSql.AppendFormat("SELECT User_Name UserName FROM Base_UserInfo where user_id='{0}'",operator_id);
            }
            else if(operator_type=="2")
            {
                strSql.AppendFormat("SELECT UserName FROM YR_OPUser where UserID='{0}'", operator_id);
            }
            DataTable dt= DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            if(dt!=null && dt.Rows.Count>0)
            {
                return dt.Rows[0]["UserName"].ToString();
            }
            else
            {
                return "";
            }
        }

    }
}
