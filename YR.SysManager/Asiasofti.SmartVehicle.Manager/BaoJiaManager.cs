using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.Busines;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 宝驾互联互通接口数据操作类
    /// </summary>
    public class BaoJiaManager
    {
        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool OrderVehicle(Hashtable param)
        {
            StringBuilder[] sqls = new StringBuilder[3];
            object[] objs = new object[3];

            //更新车辆状态
            StringBuilder sql_upvechicle = new StringBuilder();
            sql_upvechicle.Append("UPDATE yr_vehicles SET USESTATE=").Append(VehicleUseState.Order.GetHashCode().ToString()).Append(" WHERE USESTATE=").Append(VehicleUseState.Free.GetHashCode().ToString()).Append(" AND VEHICLESTATE=").Append(VehicleState.Use.GetHashCode().ToString()).Append(" AND ID=@ID");
            SqlParam[] parm_upvechicle = new SqlParam[] { new SqlParam("@ID", param["VID"]) };
            sqls[0] = sql_upvechicle;
            objs[0] = parm_upvechicle;

            //添加订单
            StringBuilder sql_addorder = new StringBuilder();
            sql_addorder.Append(@"INSERT INTO [dbo].[YR_Orders]([ID],[OrderNum],[UserID],[VehicleID],[KMPrice],[MinutePrice],[Mileage],[Minutes],[UserPrivilegeRatio],[RaiseCommissionRatio],[PayState],[OrderState],[CreateTime],[OrderRemark],RentType,RentDuration,CityID,OrderSource,FromParking)
     VALUES
           (@ID,@OrderNum,@UserID,@VehicleID,@KMPrice,@MinutePrice,@Mileage,@Minutes,@UserPrivilegeRatio,@RaiseCommissionRatio,@PayState,@OrderState,@CreateTime,@OrderRemark,@RentType,@RentDuration,@CityID,@OrderSource,@FromParking)");
            SqlParam[] param_addordeer = new SqlParam[] { new SqlParam("@ID", param["OID"]), 
                new SqlParam("@OrderNum", param["OrderNum"]),
            new SqlParam("@UserID",param["UserID"]),
            new SqlParam("@VehicleID",param["VehicleID"]),
            new SqlParam("@KMPrice",param["KMPrice"]),
            new SqlParam("@MinutePrice",param["MinutePrice"]),
            new SqlParam("@Mileage",param["Mileage"]),
            new SqlParam("@Minutes",param["Minutes"]),
            new SqlParam("@UserPrivilegeRatio",param["UserPrivilegeRatio"]),
            new SqlParam("@RaiseCommissionRatio",param["RaiseCommissionRatio"]),
            new SqlParam("@PayState",param["PayState"]),
            new SqlParam("@OrderState",param["OrderState"]),
            new SqlParam("@CreateTime",param["CreateTime"]),
            new SqlParam("@OrderRemark",param["OrderRemark"]),
            new SqlParam("@CityID",param["CityID"]),
            new SqlParam("@OrderSource",param["OrderSource"]),
            new SqlParam("@FromParking",param["FromParking"]),
            new SqlParam("@RentType",param["RentType"]),
            new SqlParam("@RentDuration",param["RentDuration"])};
            sqls[1] = sql_addorder;
            objs[1] = param_addordeer;

            //添加订单用户
            StringBuilder sql_addOrderUser = new StringBuilder();
            sql_addOrderUser.Append(@"insert into YR_OrderUser(OrderNum,RealName,BindPhone,IDCardNum,OutOrderNo)values(@OrderNum,@RealName,@BindPhone,@IDCardNum,@OutOrderNo)");
            SqlParam[] param_addOrderUser = new SqlParam[] { new SqlParam("@OrderNum", param["OrderNum"]), 
                new SqlParam("@RealName", param["RealName"]),
                new SqlParam("@BindPhone", param["BindPhone"]),
                new SqlParam("@IDCardNum",param["IDCardNum"]),
                new SqlParam("@OutOrderNo",param["OutOrderNo"])};
            sqls[2] = sql_addOrderUser;
            objs[2] = param_addOrderUser;

            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls, objs);
        }

        /// <summary>
        /// 根据用户判断是否存在未支付订单 true:存在 false ：不存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsExistUnPayOrderByUser(string mobile)
        {
            int count = 1;
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(1) FROM YR_ORDERS join YR_OrderUser on YR_ORDERS.OrderNum=YR_OrderUser.OrderNum where YR_OrderUser.BindPhone=@BindPhone and OrderState=").Append(OrderState.UnPay.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@BindPhone";
            sqlp.FiledValue = mobile;
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
        /// 根据用户判断是否存在车辆预约订单 true:存在预约订单 false ：不存在预约订单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsExistVehicleOrderByUser(string mobile)
        {
            int count = 1;
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(1) FROM YR_ORDERS join YR_OrderUser on YR_ORDERS.OrderNum=YR_OrderUser.OrderNum where YR_OrderUser.BindPhone=@BindPhone and OrderState=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@BindPhone";
            sqlp.FiledValue = mobile;
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
        /// 根据外部订单号获取订单信息
        /// </summary>
        /// <param name="out_order_no"></param>
        /// <returns></returns>
        public Hashtable GetOrderInfo(string out_order_no)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select a.*,b.RealName,b.BindPhone,b.IDCardNum,b.OutOrderNo from yr_orders a join yr_orderuser b on a.ordernum=b.ordernum where b.OutOrderNo=@OutOrderNo");
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@OutOrderNo";
            sqlp.FiledValue = out_order_no;
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

        public bool CancelOrder(string orderid)
        {
            try
            {
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

                    //更改订单状态
                    StringBuilder sbUpdateOrder = new StringBuilder();
                    sbUpdateOrder.Append(@"UPDATE yr_orders SET TotalMoney=@TotalMoney,Minutes=@Minutes,SettlementMoney=@SettlementMoney,PayMoney=@PayMoney,PayState=@PayState,OrderState=@OrderState,EndTime=@EndTime,FinishedTime=@FinishedTime,OrderRemark=@OrderRemark where ID=@ID and OrderState=" + OrderState.Valid.GetHashCode());
                    SqlParam[] updateOrderParam = new SqlParam[] { new SqlParam("@TotalMoney", "0.00"),
                     new SqlParam("@Minutes", "0"),
                     new SqlParam("@SettlementMoney", "0.00"),
                     new SqlParam("@PayMoney", "0.00"),
                     new SqlParam("@EndTime", DateTime.Now),
                     new SqlParam("@PayState", OrderPayState.NotPay.GetHashCode()),
                     new SqlParam("@OrderState", OrderState.Invalid.GetHashCode()),
                     new SqlParam("@FinishedTime",DateTime.Now),
                     new SqlParam("@OrderRemark", "订单取消"),
                     new SqlParam("@ID", orderid)};
                    sqls.Add(sbUpdateOrder);
                    objs.Add(updateOrderParam);

                    return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ReturnVehicle(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    decimal payMoney = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "PayMoney"), out payMoney);

                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object> objs = new List<object>();

                    //更改车辆状态
                    StringBuilder sbUpdateVehicle = new StringBuilder();
                    sbUpdateVehicle.Append("UPDATE yr_vehicles SET UseState=@UseState,ParkingID=@ParkingID WHERE ID=@ID");
                    SqlParam[] updateVehicleParam = new SqlParam[] { 
                        new SqlParam("@ID", ht["VID"]),
                        new SqlParam("@ParkingID", ht["ReturnParkingID"]),
                        new SqlParam("@UseState",VehicleUseState.Free.GetHashCode())
                    };
                    sqls.Add(sbUpdateVehicle);
                    objs.Add(updateVehicleParam);


                    //更改订单状态
                    StringBuilder sbUpdateOrder = new StringBuilder();
                    sbUpdateOrder.Append(@"UPDATE yr_orders SET TotalMoney=@TotalMoney,Minutes=@Minutes,SettlementMoney=@SettlementMoney,PayMoney=@PayMoney,PayState=@PayState,OrderState=@OrderState,EndTime=@EndTime,FinishedTime=@FinishedTime,OrderRemark=@OrderRemark,OutServiceAreaFee=@OutServiceAreaFee,ReturnLocType=@ReturnLocType,ToParking=@ToParking where ID=@ID and OrderState=" + OrderState.Valid.GetHashCode());
                    SqlParam[] updateOrderParam = new SqlParam[] { new SqlParam("@TotalMoney", ht["TotalMoney"]),
                     new SqlParam("@Minutes", ht["Minutes"]),
                     new SqlParam("@SettlementMoney", ht["SettlementMoney"]),
                     new SqlParam("@PayMoney", ht["PayMoney"]),
                     new SqlParam("@EndTime", ht["EndTime"]),
                     new SqlParam("@PayState", payMoney>0?OrderPayState.NotPay.GetHashCode():OrderPayState.AlreadyPay.GetHashCode()),
                     new SqlParam("@OrderState", payMoney>0?OrderState.UnPay.GetHashCode():OrderState.Finished.GetHashCode()),
                     new SqlParam("@FinishedTime", payMoney>0?null:ht["EndTime"]),
                     new SqlParam("@OrderRemark", ht["Remark"]),
                     new SqlParam("@OutServiceAreaFee", ht["OutServiceAreaFee"]),
                     new SqlParam("@ReturnLocType", ht["ReturnLocType"]),
                     new SqlParam("@ToParking", ht["ReturnParkingID"]),
                     new SqlParam("@ID", ht["OID"])};
                    sqls.Add(sbUpdateOrder);
                    objs.Add(updateOrderParam);

                    return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Hashtable GetUserCurrentOrder(string mobile)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select a.*,b.RealName,b.BindPhone,b.IDCardNum,b.OutOrderNo from yr_orders a join yr_orderuser b on a.ordernum=b.ordernum where b.BindPhone=@BindPhone and a.and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@BindPhone";
            sqlp.FiledValue = mobile;
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

        public bool OrderPayCallback(Hashtable ht)
        {
            try
            {
                OrdersManager om = new OrdersManager();
                VehicleManager vm = new VehicleManager();
                SysSettingManager settingManager = new SysSettingManager();

                string ordernum = ht["OrderNum"].ToString();
                decimal paymoney = 0.00m;
                decimal.TryParse(ht["TotalFee"].ToString(), out paymoney);
                Hashtable orderinfo = GetOrderInfo(ordernum);
                
                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                //更改订单状态
                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append(@"UPDATE yr_orders SET PayMoney=@PayMoney,PayTime=@PayTime,FinishedTime=@FinishedTime,PayState=@PayState,OrderState=@OrderState where OrderNum=@OrderNum");
                SqlParam[] updateOrderParam = new SqlParam[] {
                     new SqlParam("@PayMoney", paymoney),
                     new SqlParam("@PayState", OrderPayState.AlreadyPay.GetHashCode()),
                     new SqlParam("@OrderState", OrderState.Finished.GetHashCode()),
                     new SqlParam("@PayTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@FinishedTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@OrderNum", ordernum)};
                sqls.Add(sbUpdateOrder);
                objs.Add(updateOrderParam);

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        public bool SetCurrentOrderStartTime(string orderid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select id from yr_orders where ID=@ID and starttime is null and orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = orderid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt != null && dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE yr_orders SET StartTime=GETDATE() WHERE ID='" + orderid + "' and orderstate=" + OrderState.Valid.GetHashCode().ToString());
                int iNum = DataFactory.SqlDataBase().ExecuteBySql(sb);
                return iNum > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 获取待取消的订单
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public DataTable GetAwaitCancelOrder(int minutes)
        {
            StringBuilder sql = new StringBuilder();
            //sql.Append(@"select a.*,b.RealName,b.BindPhone,b.IDCardNum,b.OutOrderNo from yr_orders a join yr_orderuser b on a.ordernum=b.ordernum where a.StartTime is null and a.OrderSource='02' and a.CreateTime<@CancelTime and a.orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            sql.Append(@"select a.*,b.RealName,b.BindPhone,b.IDCardNum,b.OutOrderNo from yr_orders a left join yr_orderuser b on a.ordernum=b.ordernum where a.StartTime is null and a.CreateTime<@CancelTime and a.orderstate=").Append(OrderState.Valid.GetHashCode().ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@CancelTime";
            sqlp.FiledValue = DateTime.Now.AddMinutes(-minutes);
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }
    }
}
