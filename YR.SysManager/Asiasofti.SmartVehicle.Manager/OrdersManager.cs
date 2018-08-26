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
using YR.Common.DotNetLog;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 订单管理 
    /// </summary>
    public class OrdersManager
    {
        /// <summary>
        /// 添加或修改订单信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditOrdersInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Orders", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除订单信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelOrdersInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_Orders", "ID", _key, ht);
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
            //strSql.Append(@"select a.*,b.BindPhone,b.RealName,b.NickName,c.Name AS VehicleName,c.LicenseNumber as LicenseNumber from YR_Orders a LEFT JOIN YR_UserInfo b ");
            //strSql.Append(" ON a.UserID=b.ID LEFT JOIN YR_Vehicles c ON a.VehicleID=c.ID where 1=1");
            strSql.Append(@"select a.*,b.BindPhone,b.RealName,b.OutOrderNo,c.Name AS VehicleName,c.LicenseNumber as LicenseNumber from YR_Orders a LEFT JOIN view_OrderUser b ");
            strSql.Append(" ON a.ID=b.ID LEFT JOIN YR_Vehicles c ON a.VehicleID=c.ID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }



        public DataTable GetOrdersReport(string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select t1.ReportTime,OrderCount,TotalIncome,PaymentCount,PaidCount,Income,VehicleCount,PayVehicleCount,PayNormalVehicleCount,StartCount,EndCount,NormalCount,ISNULL(AbnormalCount,0) AbnormalCount,ISNULL(CancelCount,0) CancelCount from(select convert(char(10),CreateTime,121) ReportTime,count(1) OrderCount,sum(PayMoney) TotalIncome from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121)) t1 ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) PaymentCount,sum(PayMoney) Income from YR_Orders where PayState=1 and CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121)) t2 on t2.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select t.ReportTime,count(t.VehicleID) PayVehicleCount from(select convert(char(10),CreateTime,121) ReportTime,VehicleID from YR_Orders where PayState=1 and CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121),VehicleID) t group by t.ReportTime) t3 on t3.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) StartCount from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime and StartTime is not null group by convert(char(10),CreateTime,121)) t4 on t4.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) NormalCount from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime and Mileage>=0.2 group by convert(char(10),CreateTime,121)) t5 on t5.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) AbnormalCount from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime and OrderRemark='异常还车(免费)' group by convert(char(10),CreateTime,121)) t6 on t6.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) EndCount from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime and EndTime is not null group by convert(char(10),CreateTime,121)) t7 on t7.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) CancelCount from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime and OrderRemark='客户取消订单' group by convert(char(10),CreateTime,121)) t8 on t8.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select t.ReportTime,count(t.VehicleID) VehicleCount from(select convert(char(10),CreateTime,121) ReportTime,VehicleID from YR_Orders where CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121),VehicleID) t group by t.ReportTime) t9 on t9.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select t.ReportTime,count(t.VehicleID) PayNormalVehicleCount from(select convert(char(10),CreateTime,121) ReportTime,VehicleID from YR_Orders where PayState=1 and PayMoney>=2 and CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121),VehicleID) t group by t.ReportTime) t10 on t10.ReportTime=t1.ReportTime ");
            strSql.Append(@"left join(select convert(char(10),CreateTime,121) ReportTime,count(1) PaidCount from YR_Orders where PayState=1 and PayMoney>=2 and CreateTime>=@StartTime and CreateTime<=@EndTime group by convert(char(10),CreateTime,121)) t11 on t11.ReportTime=t1.ReportTime ");
            strSql.Append(@"order by t1.ReportTime desc");
            SqlParam param1 = new SqlParam();
            param1.FieldName = "@StartTime";
            param1.FiledValue = startTime;
            SqlParam param2 = new SqlParam();
            param2.FieldName = "@EndTime";
            param2.FiledValue = endTime;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param1, param2 });
            return dt;
        }

        public DataTable FinanceReport(string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" select t1.PayTime,t1.OrderPayMoney,t2.ChangesAmount,t3.DepositMoney,t4.ReturnMoney from( ");
            strSql.Append(@"select convert(char(10),PayTime,121) PayTime,sum(PayMoney) OrderPayMoney from YR_Orders ");
            strSql.Append(@"where PayState = 1 and OrderState = 2 ");
            strSql.Append(@"and PayTime>=@StartTime ");
            strSql.Append(@"and PayTime<=@EndTime ");
            strSql.Append(@"group by convert(char(10), PayTime, 121) ");
            strSql.Append(@") t1 ");
            strSql.Append(@"left join( ");
            strSql.Append(@"select convert(char(10), ChangesTime, 121) PayTime, sum(ChangesAmount) ChangesAmount from YR_UserFinancial ");
            strSql.Append(@"where State = 2 and ChangesType = 1 ");
            strSql.Append(@"and ChangesTime >=@StartTime ");
            strSql.Append(@"and ChangesTime <=@EndTime ");
            strSql.Append(@"group by convert(char(10), ChangesTime, 121) ");
            strSql.Append(@") t2 on t2.PayTime = t1.PayTime ");
            strSql.Append(@"left join( ");
            strSql.Append(@"select convert(char(10),ChangesTime,121) PayTime,sum(ChangesAmount) DepositMoney from YR_UserFinancial ");
            strSql.Append(@"where State = 2 and ChangesType = 5 ");
            strSql.Append(@"and ChangesTime>=@StartTime ");
            strSql.Append(@"and ChangesTime<=@EndTime ");
            strSql.Append(@"group by convert(char(10), ChangesTime, 121) ");
            strSql.Append(@") t3 on t3.PayTime = t1.PayTime ");
            strSql.Append(@"left join( ");
            strSql.Append(@"select convert(char(10),UpdateTime,121) PayTime,sum(DepositMoney) ReturnMoney from YR_DepositReturn ");
            strSql.Append(@"where State = 2 ");
            strSql.Append(@"and UpdateTime>=@StartTime ");
            strSql.Append(@"and UpdateTime<=@EndTime ");
            strSql.Append(@"group by convert(char(10), UpdateTime, 121) ");
            strSql.Append(@") t4 on t4.PayTime = t1.PayTime ");
            strSql.Append(@"order by PayTime desc ");
            SqlParam param1 = new SqlParam();
            param1.FieldName = "@StartTime";
            param1.FiledValue = startTime;
            SqlParam param2 = new SqlParam();
            param2.FieldName = "@EndTime";
            param2.FiledValue = endTime;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param1, param2 });
            return dt;
        }

        public DataTable VehicleStatistics(string startTime, string endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select t1.PayTime,t1.OrderCount,t2.VehicleCount from( ");
            strSql.Append(@"select convert(char(10),PayTime,121) PayTime,count(1) OrderCount from YR_Orders  ");
            strSql.Append(@"where PayState = 1 and OrderState = 2 ");
            strSql.Append(@"and PayTime>=@StartTime ");
            strSql.Append(@"and PayTime<=@EndTime ");
            strSql.Append(@"group by convert(char(10), PayTime, 121) ");
            strSql.Append(@") t1 ");
            strSql.Append(@"left join ( ");
            strSql.Append(@"select convert(char(10),PayTime,121) PayTime,count(DISTINCT VehicleID) VehicleCount from YR_Orders ");
            strSql.Append(@"where PayState = 1 and OrderState = 2 ");
            strSql.Append(@"and PayTime>=@StartTime ");
            strSql.Append(@"and PayTime<=@EndTime ");
            strSql.Append(@"group by convert(char(10), PayTime, 121) ");
            strSql.Append(@") t2 on t2.PayTime=t1.PayTime order by PayTime desc ");
            SqlParam param1 = new SqlParam();
            param1.FieldName = "@StartTime";
            param1.FiledValue = startTime;
            SqlParam param2 = new SqlParam();
            param2.FieldName = "@EndTime";
            param2.FiledValue = endTime;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql, new SqlParam[] { param1, param2 });
            return dt;
        }

        /// <summary>
        /// 根据用户分页获取用户订单信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetOrdersInfoByUser(string uid, int currentPage, int pageNum)
        {
            StringBuilder strsql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strsql.Append("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CreateTime desc ) as RowNum, ID,OrderNum,UserID,VehicleID,dbo.GET_VEHICLENAME(VehicleID) as VehicleName,dbo.GetLicenseNumber(VehicleID) as LicenseNumber,StartTime,StartAddress,EndTime,EndAddress,KMPrice,MinutePrice,Mileage,Minutes,TotalMoney,SettlementMoney,PayMoney,PayState,PayRemark,PayTime,OrderState,OrderRemark,FinishedTime,OutServiceAreaFee from yr_orders where UserID=@UserID) as  Orders where RowNum between ").Append(start.ToString()).Append(" and ").Append(end);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strsql, new SqlParam[] { sqlp });
            return dt;
        }
        /// <summary>
        /// 根据车辆分页获取用户订单信息
        /// </summary>
        /// <param name="vehicleid"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public DataTable GetOrdersInfoByVehicle(string vehicleid, int currentPage, int pageNum)
        {
            StringBuilder strsql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            strsql.Append("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CreateTime desc ) as RowNum, ID,OrderNum,UserID,VehicleID,StartTime,StartAddress,EndTime,EndAddress,KMPrice,MinutesPrice,Mileage,Minutes,UserPrivilegeRatio,SettlementMoney,PayMoney,PayState,PayRemark,PayTime,OrderState,OrderRemark,FinishedTime,RaiseCommissionRatio,RaiseCommissionMoney,RentType,RentDuration from yr_orders where VehicleID=@VehicleID) as  Orders where RowNum between ").Append(start.ToString()).Append(" and ").Append(end);
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@VehicleID";
            sqlp.FiledValue = vehicleid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strsql, new SqlParam[] { sqlp });
            return dt;
        }

        public DataTable GetOrdersInfoByVehicle(string vehicleid, DateTime? begin, DateTime? end, int currentPage, int pageNum)
        {
            int count = 0;
            StringBuilder strSql = new StringBuilder();
            //strSql.Append(@"select a.*,b.BindPhone,b.RealName,b.NickName,c.Name AS VehicleName from YR_Orders a LEFT JOIN YR_UserInfo b ");
            //strSql.Append(" ON a.UserID=b.ID LEFT JOIN YR_Vehicles c ON a.VehicleID=c.ID where 1=1");
            //strSql.Append(" and a.VehicleID=@VehicleID and a.CreateTime between @Begin and @End");
            strSql.Append(@"select a.*,b.BindPhone,b.RealName,b.OutOrderNo,c.Name AS VehicleName from YR_Orders a LEFT JOIN view_OrderUser b ");
            strSql.Append(" ON a.ID=b.ID LEFT JOIN YR_Vehicles c ON a.VehicleID=c.ID where 1=1");
            strSql.Append(" and a.VehicleID=@VehicleID and a.CreateTime between @Begin and @End");
            SqlParam sqlp1 = new SqlParam();
            sqlp1.FieldName = "@VehicleID";
            sqlp1.FiledValue = vehicleid;
            SqlParam sqlp2 = new SqlParam();
            sqlp2.FieldName = "@Begin";
            sqlp2.FiledValue = begin.HasValue ? begin.Value : DateTime.MinValue;
            SqlParam sqlp3 = new SqlParam();
            sqlp3.FieldName = "@End";
            sqlp3.FiledValue = end.HasValue ? end.Value : DateTime.MaxValue;
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), new SqlParam[] { sqlp1, sqlp2, sqlp3 }, "CreateTime", "Desc", currentPage, pageNum, ref count);
        }

        /// <summary>
        /// 根据id得到订单信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Hashtable GetOrderInfoByID(string oid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from yr_orders where ID=@ID");
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
        /// 根据用户id获取当前的未结算订单信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetCurrentNotPayOrderInfoByUserID(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top 1 * from yr_orders where UserID=@UserID and PayState=").Append(OrderPayState.NotPay.GetHashCode()).Append(" and OrderState=").Append(OrderState.Valid.GetHashCode());
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
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
        public Hashtable GetUserCurrentOrderVehicle(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select yr_vehicles.ID as VID,yr_vehicles.VehicleGPSNum,yr_vehicles.CityID,yr_vehicles.UserID,yr_vehicles.LicenseNumber,yr_vehicles.[Name],yr_vehicles.Model,dbo.GetModelName(yr_vehicles.Model) ModelName,yr_vehicles.Longitude,yr_vehicles.Latitude,yr_vehicles.Address,yr_vehicles.ExpectRange,yr_vehicles.Electricity,yr_vehicles.[Range],yr_vehicles.ExpectHighestSpeed,yr_vehicles.CurrentSpeed,yr_vehicles.LockState,yr_orders.KMPrice,yr_orders.MinutePrice,yr_orders.Mileage,yr_orders.Minutes,yr_orders.CreateTime,yr_orders.StartTime,yr_orders.ID as orderid,yr_orders.OrderNum,yr_orders.TotalMoney,yr_orders.SettlementMoney,yr_orders.PayMoney from yr_orders left join yr_vehicles on yr_orders.vehicleid = yr_vehicles.id where yr_orders.userid=@UserID and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
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

        /// <summary>
        /// 设置当前订单的开始时间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool SetCurrentOrderStartTime(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select id from yr_orders where UserID=@UserID and starttime is null and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE yr_orders SET StartTime=GETDATE() WHERE UserID='" + uid + "' and orderstate=" + OrderState.Valid.GetHashCode().ToString());
                int iNum = DataFactory.SqlDataBase().ExecuteBySql(sb);
                return iNum > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据订单预约车辆当前的坐标更新订单的开始坐标或结束坐标
        /// </summary>
        /// <param name="orderID">订单id</param>
        /// <param name="isStart">是否为起始坐标，true为起点坐标，false为终点坐标</param>
        /// <returns></returns>
        public bool UpdateOrderGPS(string orderID, bool isStart)
        {
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();

            if (isStart)
            {
                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append("update YR_Orders set StartLongitude=Longitude,StartLatitude=Latitude from YR_Vehicles where YR_Orders.vehicleid=YR_Vehicles.id and YR_Orders.id=@OrderID");
                SqlParam[] parmUpdateOrder = new SqlParam[] {
                        new SqlParam("@OrderID", orderID)
                    };
                sqls.Add(sbUpdateOrder);
                objs.Add(parmUpdateOrder);
            }
            else
            {
                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append("update YR_Orders set EndLongitude=Longitude,EndLatitude=Latitude from YR_Vehicles where YR_Orders.vehicleid=YR_Vehicles.id and YR_Orders.id=@OrderID");
                SqlParam[] parmUpdateOrder = new SqlParam[] {
                        new SqlParam("@OrderID", orderID)
                    };
                sqls.Add(sbUpdateOrder);
                objs.Add(parmUpdateOrder);
            }
            return DataFactory.SqlDataBase().BatchExecuteBySql(sqls.ToArray(), objs.ToArray()) >= 0 ? true : false;
        }
        /// <summary>
        /// 得到用户当前预约的车辆信息GPS编号
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUserCurrentOrderVehicleGPSNum(string uid)
        {
            StringBuilder sql = new StringBuilder();
            //sql.Append(@"select yr_vehicles.ID as VID,yr_vehicles.VehicleGPSNum from yr_orders left join yr_vehicles on yr_orders.vehicleid = yr_vehicles.id where yr_orders.userid=@UserID and paystate=").Append(OrderPayState.NotPay.GetHashCode().ToString()).Append(" and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            sql.Append(@"select yr_vehicles.ID as VID,yr_vehicles.VehicleGPSNum,yr_orders.OrderNum,yr_orders.StartTime from yr_orders left join yr_vehicles on yr_orders.vehicleid = yr_vehicles.id where yr_orders.userid=@UserID and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
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
        /// <summary>
        /// 根据用户判断是否存在车辆预约订单 true:存在预约订单 false ：不存在预约订单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsExistVehicleOrderByUser(string uid)
        {
            int count = 1;
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(ID) FROM YR_ORDERS where USERID=@UserID and OrderState=").Append(OrderState.Valid.GetHashCode().ToString()).Append(" and PayState=").Append(OrderPayState.NotPay.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            else
            {
                int.TryParse(dt.Rows[0][0].ToString(), out count);
                if (count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 根据用户判断是否存在未支付订单 true:存在 false ：不存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsExistUnPayOrderByUser(string uid)
        {
            int count = 1;
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(ID) FROM YR_ORDERS where USERID=@UserID and OrderState=").Append(OrderState.UnPay.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            else
            {
                int.TryParse(dt.Rows[0][0].ToString(), out count);
                if (count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Hashtable GetOrderInfoByNum(string ordernum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from yr_orders where OrderNum=@OrderNum");
            SqlParam param = new SqlParam();
            param.FieldName = "@OrderNum";
            param.FiledValue = ordernum;
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
        /// 订单计费时长，单位：分钟
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public int GetOrderBillingTime(string ordernum)
        {
            int minutes = 0;

            Hashtable order = GetOrderInfoByNum(ordernum);
            string orderState = SiteHelper.GetHashTableValueByKey(order, "OrderState");
            if (orderState == "1")
            {
                SysSettingManager settingManager = new SysSettingManager();
                int OrderDelayMinutes = 15;
                int.TryParse(settingManager.GetValueByKey("OrderDelayMinutes"), out OrderDelayMinutes);

                DateTime createtime = DateTime.Now;
                DateTime startTime = DateTime.Now;
                string strStartTime = SiteHelper.GetHashTableValueByKey(order, "StartTime");
                DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "CreateTime"), out createtime);
                DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "StartTime"), out startTime);
                DateTime billingTime = createtime.AddMinutes(OrderDelayMinutes);
                if (!string.IsNullOrEmpty(strStartTime))
                {
                    if (SiteHelper.ExecDateDiff(createtime, startTime) < OrderDelayMinutes)
                    {
                        billingTime = startTime;
                    }
                }
                if (billingTime <= DateTime.Now)
                {
                    minutes = SiteHelper.ExecDateDiff(billingTime, DateTime.Now);
                }
            }
            else
            {
                int.TryParse(SiteHelper.GetHashTableValueByKey(order, "Minutes"), out minutes);
            }
            return minutes;
        }

        /// <summary>
        /// 根据订单ID获取订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetOrderByNum(string ordernum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select yr_orders.ID,yr_orders.OrderNum,yr_orders.UserID,VehicleID,dbo.GET_VEHICLENAME(VehicleID) as VehicleName,dbo.GetLicenseNumber(VehicleID) as LicenseNumber,StartTime,StartAddress,EndTime,EndAddress,KMPrice,MinutePrice,Mileage,Minutes,SettlementMoney,PayMoney,TotalMoney,PayState,PayRemark,PayTime,OrderState,OrderRemark,FinishedTime,CreateTime,OperatorWay,OutServiceAreaFee from yr_orders  left join YR_UserFinancial on yr_orders.OrderNum=YR_UserFinancial.OrderNum where yr_orders.OrderNum=@OrderNum");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@OrderNum";
            sqlp.FiledValue = ordernum;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataTableToHashtable(dt);
            }
        }

        /// <summary>
        /// 根据用户ID获取未支付订单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUnPaidOrderByUser(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top(1) ID,OrderNum,UserID,VehicleID,dbo.GET_VEHICLENAME(VehicleID) as VehicleName,dbo.GetLicenseNumber(VehicleID) as LicenseNumber,StartTime,StartAddress,EndTime,EndAddress,KMPrice,MinutePrice,Mileage,Minutes,SettlementMoney,PayMoney,TotalMoney,PayState,PayRemark,PayTime,OrderState,OrderRemark,FinishedTime,CreateTime,OutServiceAreaFee from yr_orders where UserID=@UserID and orderstate=").Append(OrderState.UnPay.GetHashCode().ToString()).Append(" order by CreateTime desc"); ;
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


        /// <summary>
        /// 根据车辆id获取最近一次用车用户信息
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public Hashtable GetLatestUserByVehicleID(string vid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top 1 * from");
            sql.Append(@"(");
            //sql.Append(@"SELECT UserID,1 UserType,CreateTime,RealName,HeadPortrait,BindPhone,OrderState FROM YR_Orders join YR_UserInfo on YR_Orders.UserID=YR_UserInfo.ID where VehicleID='" + vid + "'");
            sql.Append(@"SELECT UserID,1 UserType,CreateTime,RealName,'' HeadPortrait,BindPhone,OrderState FROM YR_Orders join view_OrderUser on YR_Orders.ID=view_OrderUser.ID where VehicleID='" + vid + "'");
            sql.Append(@" union ");
            sql.Append(@"SELECT YR_OPLog.UserID,2 UserType,OperateTime,UserName,HeadPortrait,Tel,null FROM YR_OPLog join YR_OPUser on YR_OPLog.UserID=YR_OPUser.UserID where VehicleID='" + vid + "' and OperateType=1");
            sql.Append(@") t order by CreateTime desc");
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

        public Hashtable GetLatestUserOrder(string vid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT top 1 YR_Orders.ID,OrderNum,StartTime,Mileage,TotalMoney,SettlementMoney,Insurance,PlatIncome,PayMoney,PayState,PayTime,OrderState,CreateTime,FinishedTime,UserID,UserName,NickName,RealName,BindPhone,HeadPortrait FROM YR_Orders join YR_UserInfo on YR_Orders.UserID=YR_UserInfo.ID where YR_Orders.VehicleID='").Append(vid).Append("' order by CreateTime desc");
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

        public Hashtable GetInUseOrder(string vid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select top 1 ID,OrderNum,UserID,VehicleID,StartTime,OrderState,CreateTime from YR_Orders where OrderState in(1, 2) and VehicleID='")
                .Append(vid)
                .Append("' order by CreateTime desc");
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
        /// 获取订单车辆还车提醒列表
        /// </summary>
        /// <param name="minutes_timeout">超时分钟数</param>
        /// <returns></returns>
        public DataTable GetVehicleReturnOrders(int minutes_timeout)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT YR_Orders.id OrderID,YR_Vehicles.ID,YR_Vehicles.Name,YR_Vehicles.LicenseNumber,YR_UserInfo.PushID FROM YR_Orders join YR_Vehicles on YR_Orders.VehicleID=YR_Vehicles.ID join YR_UserInfo on YR_Orders.userid=YR_UserInfo.id where YR_Vehicles.LockState='lock' and orderstate=1 and YR_Orders.CreateTime<'{0}'", DateTime.Now.AddMinutes(-minutes_timeout).ToString("yyyyMMdd HH:mm:ss"));
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            return dt;
        }

        /// <summary>
        /// 获取订单车辆还车提醒列表
        /// </summary>
        /// <param name="minutes_timeout">超时分钟数</param>
        /// <returns></returns>
        public DataTable GetVehicleReturnOrderList(int minutes_timeout)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select ID,OrderNum,UserID,VehicleID,OrderState,StartTime,EndTime from YR_Orders where OrderState=1 and EndTime is null and StartTime is not null and DATEDIFF(mi, StartTime, getdate())>{0}", minutes_timeout);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            return dt;
        }


        /// <summary>
        /// 获取待取消的订单
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public DataTable GetAwaitCancelOrder(int minutes)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select a.*,b.RealName,b.BindPhone,b.IDCardNum from yr_orders a left join YR_UserInfo b on a.UserID=b.ID where a.StartTime is null and a.CreateTime<@CancelTime and a.orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@CancelTime";
            sqlp.FiledValue = DateTime.Now.AddMinutes(-minutes);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }

        /// <summary>
        /// 根据订单号取消订单
        /// </summary>
        /// <param name="OrderNum"></param>
        /// <returns></returns>
        public bool CancelOrder(string orderNum, int type)
        {
            Log Logger = LogFactory.GetLogger(typeof(OrdersManager));
            try
            {
                VehicleManager vehicleManager = new VehicleManager();
                Hashtable orderInfo = GetOrderByNum(orderNum);
                string orderState = SiteHelper.GetHashTableValueByKey(orderInfo, "OrderState");
                string startTime = SiteHelper.GetHashTableValueByKey(orderInfo, "StartTime");
                if (orderInfo != null && orderInfo.Keys.Count > 0 && orderState == "1" && string.IsNullOrEmpty(startTime))
                {
                    string orderid = SiteHelper.GetHashTableValueByKey(orderInfo, "ID");
                    string vid = SiteHelper.GetHashTableValueByKey(orderInfo, "VehicleID");
                    Hashtable vehicleInfo = vehicleManager.GetVehicleInfoByID(vid);
                    string currentSpeed = SiteHelper.GetHashTableValueByKey(vehicleInfo, "CurrentSpeed");
                    decimal speed = 0m;
                    decimal.TryParse(currentSpeed, out speed);
                    Logger.Warn("订单ID:" + orderid + ",订单号:" + orderNum + "，车辆ID:" + vid + "，StartTime 为空，速度:" + speed + "，自动取消订单");
                    vehicleManager.CloseVehicle(vid);

                    if (!string.IsNullOrEmpty(orderid))
                    {
                        List<StringBuilder> sqls = new List<StringBuilder>();
                        List<object> objs = new List<object>();

                        //更改车辆状态
                        StringBuilder sbUpdateVehicle = new StringBuilder();
                        sbUpdateVehicle.Append("UPDATE yr_vehicles SET UseState=@UseState from yr_orders WHERE yr_orders.VehicleID=yr_vehicles.ID and yr_orders.ID=@ID");
                        SqlParam[] updateVehicleParam = new SqlParam[] {
                            new SqlParam("@ID", orderid),
                            new SqlParam("@UseState",VehicleUseState.Free.GetHashCode())
                        };
                        sqls.Add(sbUpdateVehicle);
                        objs.Add(updateVehicleParam);

                        string remark = "客户取消订单";
                        if (type == 1)
                        {
                            remark = "系统自动取消订单";
                        }
                        //更改订单状态
                        StringBuilder sbUpdateOrder = new StringBuilder();
                        sbUpdateOrder.Append(@"UPDATE yr_orders SET TotalMoney=@TotalMoney,Minutes=@Minutes,SettlementMoney=@SettlementMoney,PayMoney=@PayMoney,PayState=@PayState,OrderState=@OrderState,EndTime=@EndTime,FinishedTime=@FinishedTime,OrderRemark=@OrderRemark where ID=@ID and OrderState=" + OrderState.Valid.GetHashCode());
                        SqlParam[] updateOrderParam = new SqlParam[] {
                            new SqlParam("@TotalMoney", "0.00"),
                            new SqlParam("@Minutes", "0"),
                            new SqlParam("@SettlementMoney", "0.00"),
                            new SqlParam("@PayMoney", "0.00"),
                            new SqlParam("@EndTime", DateTime.Now),
                            new SqlParam("@PayState", OrderPayState.NotPay.GetHashCode()),
                            new SqlParam("@OrderState", OrderState.Invalid.GetHashCode()),
                            new SqlParam("@FinishedTime",DateTime.Now),
                            new SqlParam("@OrderRemark",remark),
                            new SqlParam("@ID", orderid)};
                        sqls.Add(sbUpdateOrder);
                        objs.Add(updateOrderParam);

                        return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
