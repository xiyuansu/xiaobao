/*
 * 订单数据操作类
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
using YR.Common.DotNetData;
using System.Collections;
using YR.Common.DotNetCode;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 充电桩订单管理 
    /// </summary>
    public class ChargingOrdersManager
    {
        /// <summary>
        /// 添加或修改订单信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditOrdersInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_ChargingOrders", "ID", _key, ht);
        }

        /// <summary>
        /// 订单信息列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetOrdersInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.BindPhone,b.RealName,b.NickName,c.Name AS ChargingPointName from YR_ChargingOrders a LEFT JOIN YR_UserInfo b ");
            strSql.Append(" ON a.UserID=b.ID LEFT JOIN YR_ChargingPoints c ON a.ChargingPointID=c.ID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 根据id得到订单信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Hashtable GetOrderInfoByID(string oid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_ChargingOrders where ID=@ID");
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = oid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
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
        /// 得到用户当前预约的车辆信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUserCurrentOrder(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT a.ID,OrderNum,ChargingPointID,UserID,TotalMoney,PayState,PayMoney,PayTime,OrderState,BeginTime,FinishTime,a.CreateTime,b.Name,b.ipaddr,b.imei,b.Longitude,b.Latitude,b.Address,datediff(minute,BeginTime,isnull(FinishTime,getdate())) TotalMinute FROM YR_ChargingOrders a join YR_ChargingPoints b on a.ChargingPointID=b.ID where a.UserID=@UserID and orderstate=1");
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


        public bool NewOrder(Hashtable param)
        {
            StringBuilder[] sqls = new StringBuilder[2];
            object[] objs = new object[2];
            //更新充电桩状态
            StringBuilder sql_upChargingPoints = new StringBuilder();
            sql_upChargingPoints.Append("UPDATE YR_ChargingPoints SET USESTATE=2 where id=@ID");
            SqlParam[] parm_upChargingPoints = new SqlParam[] { new SqlParam("@ID", param["ChargingPointID"]) };
            sqls[0] = sql_upChargingPoints;
            objs[0] = parm_upChargingPoints;

            //添加订单
            StringBuilder sql_addorder = new StringBuilder();
            sql_addorder.Append(@"INSERT INTO YR_ChargingOrders(ID,OrderNum,ChargingPointID,UserID,PayState,OrderState,BeginTime,CreateTime)
     VALUES
           (@ID,@OrderNum,@ChargingPointID,@UserID,@PayState,@OrderState,@BeginTime,@CreateTime)");
            SqlParam[] param_addordeer = new SqlParam[] { new SqlParam("@ID", param["ID"]), 
                new SqlParam("@OrderNum", param["OrderNum"]),
                new SqlParam("@ChargingPointID", param["ChargingPointID"]),
                new SqlParam("@UserID", param["UserID"]),
                new SqlParam("@PayState", param["PayState"]),
                new SqlParam("@BeginTime", param["BeginTime"]),
                new SqlParam("@OrderState", "1"),
            new SqlParam("@CreateTime",param["CreateTime"])};
            sqls[1] = sql_addorder;
            objs[1] = param_addordeer;
            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls, objs);
        }

        public bool EndOrder(Hashtable param)
        {
            StringBuilder[] sqls = new StringBuilder[2];
            object[] objs = new object[2];
            //更新充电桩状态
            StringBuilder sql_upChargingPoints = new StringBuilder();
            sql_upChargingPoints.Append("UPDATE YR_ChargingPoints SET USESTATE=1 where id=@ID");
            SqlParam[] parm_upChargingPoints = new SqlParam[] { new SqlParam("@ID", param["ChargingPointID"]) };
            sqls[0] = sql_upChargingPoints;
            objs[0] = parm_upChargingPoints;

            //线束订单
            StringBuilder sql_addorder = new StringBuilder();
            sql_addorder.Append(@"update YR_ChargingOrders set PayState=0,OrderState=@OrderState,FinishTime=@FinishTime,TotalMoney=@TotalMoney,Minutes=@Minutes where id=@ID");
            SqlParam[] param_addordeer = new SqlParam[] { new SqlParam("@ID", param["ID"]), 
                new SqlParam("@OrderState", "2"), 
                new SqlParam("@TotalMoney", param["TotalMoney"]),
                new SqlParam("@Minutes", param["Minutes"]),
                new SqlParam("@FinishTime",param["FinishTime"])};
            sqls[1] = sql_addorder;
            objs[1] = param_addordeer;
            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls, objs);
        }
    }
}
