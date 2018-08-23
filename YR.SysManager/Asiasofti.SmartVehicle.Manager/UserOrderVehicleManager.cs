using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YR.Common;
using YR.DataBase;
using YR.Busines;
using System.Collections;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetCode;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户预约车辆管理
    /// </summary>
    public class UserOrderVehicleManager
    {
        /// <summary>
        /// 用户预订车辆
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool OrderVehicle(Hashtable param)
        {
            StringBuilder[] sqls = new StringBuilder[2];
            object[] objs = new object[2];
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
            new SqlParam("@RentType",param["RentType"]),
            new SqlParam("@OrderSource",param["OrderSource"]),
            new SqlParam("@FromParking",param["FromParking"]),
            new SqlParam("@RentDuration",param["RentDuration"])};
            sqls[1] = sql_addorder;
            objs[1] = param_addordeer;
            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls, objs);
        }

        /// <summary>
        /// 用户预订车辆(提前扣款)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool OrderVehicle2(Hashtable param)
        {
            StringBuilder[] sqls = new StringBuilder[6];
            object[] objs = new object[6];

            //更新车辆状态
            StringBuilder sql_upvechicle = new StringBuilder();
            sql_upvechicle.Append("UPDATE yr_vehicles SET USESTATE=").Append(VehicleUseState.Order.GetHashCode().ToString()).Append(" WHERE USESTATE=").Append(VehicleUseState.Free.GetHashCode().ToString()).Append(" AND VEHICLESTATE=").Append(VehicleState.Use.GetHashCode().ToString()).Append(" AND ID=@ID");
            SqlParam[] parm_upvechicle = new SqlParam[] { new SqlParam("@ID", param["VID"]) };
            sqls[0] = sql_upvechicle;
            objs[0] = parm_upvechicle;

            //添加订单
            StringBuilder sql_addorder = new StringBuilder();
            sql_addorder.Append(@"INSERT INTO [dbo].[YR_Orders]([ID],[OrderNum],[UserID],[VehicleID],[KMPrice],[MinutesPrice],[Mileage],[Minutes],[UserPrivilegeRatio],[RaiseCommissionRatio],[PayMoney],[PayTime],[PayState],[OrderState],[CreateTime],[OrderRemark],TotalMoney,SettlementMoney,RaiseCommissionMoney,RentType,RentDuration)
     VALUES
           (@ID,@OrderNum,@UserID,@VehicleID,@KMPrice,@MinutesPrice,@Mileage,@Minutes,@UserPrivilegeRatio,@RaiseCommissionRatio,@PayMoney,@PayTime,@PayState,@OrderState,@CreateTime,@OrderRemark,@TotalMoney,@SettlementMoney,@RaiseCommissionMoney,@RentType,@RentDuration)");
            SqlParam[] param_addordeer = new SqlParam[] { new SqlParam("@ID", param["OID"]), 
                new SqlParam("@OrderNum", param["OrderNum"]),
            new SqlParam("@UserID",param["UserID"]),
            new SqlParam("@VehicleID",param["VehicleID"]),
            new SqlParam("@KMPrice",param["KMPrice"]),
            new SqlParam("@MinutesPrice",param["MinutesPrice"]),
            new SqlParam("@Mileage",param["Mileage"]),
            new SqlParam("@Minutes",param["Minutes"]),
            new SqlParam("@UserPrivilegeRatio",param["UserPrivilegeRatio"]),
            new SqlParam("@RaiseCommissionRatio",param["RaiseCommissionRatio"]),
            new SqlParam("@PayMoney",param["PayMoney"]),
            new SqlParam("@PayTime",param["PayTime"]),
            new SqlParam("@PayState",param["PayState"]),
            new SqlParam("@OrderState",param["OrderState"]),
            new SqlParam("@CreateTime",param["CreateTime"]),
            new SqlParam("@OrderRemark",param["OrderRemark"]),
            new SqlParam("@TotalMoney",param["TotalMoney"]),
            new SqlParam("@SettlementMoney",param["SettlementMoney"]),
            new SqlParam("@RaiseCommissionMoney",param["RaiseCommissionMoney"]),
            new SqlParam("@RentType",param["RentType"]),
            new SqlParam("@RentDuration",param["RentDuration"])};
            sqls[1] = sql_addorder;
            objs[1] = param_addordeer;

            UserInfoManager um = new UserInfoManager();
            Hashtable userinfo = um.GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(param, "UserID"));
            decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
            decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
            decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
            decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

            RechargeCardManager rcm = new RechargeCardManager();
            decimal cardBalance=rcm.GetBalance(SiteHelper.GetHashTableValueByKey(param, "UserID"));

            // 更新用户余额
            StringBuilder sbUpdateUserInfo = new StringBuilder();
            decimal changesAmount = 0.00m;
            decimal.TryParse(param["ChangesAmount"].ToString(), out changesAmount);
            changesAmount = Math.Abs(changesAmount);
            if(cardBalance>0)
            {
                StringBuilder sbUpdateCard = new StringBuilder();
                Hashtable cardht = rcm.GetUserEnableCard(SiteHelper.GetHashTableValueByKey(param, "UserID"));
                string cardid = cardht["id"].ToString();
                if(changesAmount<=cardBalance)
                {
                    sbUpdateCard.Append("update YR_RechargeCard set Balance=Balance-@ChangesAmount Where ID =@CardID");
                    SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", changesAmount), new SqlParam("@CardID", cardid) };
                    sqls[2] = sbUpdateCard;
                    objs[2] = parm;
                }
                else
                {
                    sbUpdateCard.Append("update YR_RechargeCard set Balance=0,CardState=0 Where ID =@CardID");
                    SqlParam[] parm = new SqlParam[] { new SqlParam("@CardID", cardid) };
                    sqls[2] = sbUpdateCard;
                    objs[2] = parm;      
                }
                changesAmount-=cardBalance;
            }


            if (changesAmount <= balance)
            {
                sbUpdateUserInfo.Append("update YR_UserInfo set Balance=Balance-@ChangesAmount Where ID =@UserID");
                SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", changesAmount), new SqlParam("@UserID", param["UserID"]) };
                sqls[3] = sbUpdateUserInfo;
                objs[3] = parm;
            }
            else
            {
                decimal b2 = changesAmount - balance;
                sbUpdateUserInfo.Append("update YR_UserInfo set Balance=0,Balance2=Balanc2e-@ChangesAmount Where ID =@UserID");
                SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", b2), new SqlParam("@UserID", param["UserID"]) };
                sqls[3] = sbUpdateUserInfo;
                objs[3] = parm;
            }

            // 增加财务信息
            StringBuilder balanceAdd = new StringBuilder();
            balanceAdd.Append("Insert into YR_UserFinancial(");
            balanceAdd.Append("ID,UserID,ChangesAmount,ChangesTime,ChangesType,OrderNum,Remark,Operator,OperatorType,OperatorWay,CurrentBalance,state) ");
            balanceAdd.Append("Select @ID,@UserID,@ChangesAmount,@ChangesTime,@ChangesType,@OrderID,@Remark,@Operator,@OperatorType,@OperatorWay,isnull(SUM(ChangesAmount),0)+CONVERT(DECIMAL(18,2),@ChangesAmount),@State from YR_UserFinancial WITH(NOLOCK) where UserID=@UserID and State=" + UserFinancialState.Effect.GetHashCode());
            SqlParam[] parmBalanceAdd = new SqlParam[] { 
                                     new SqlParam("@ID", param["UserFinancialID"]),
                                     new SqlParam("@UserID", param["UserID"]),
                                     new SqlParam("@ChangesAmount", param["ChangesAmount"]),
                                     new SqlParam("@ChangesTime", param["ChangesTime"]),
                                     new SqlParam("@ChangesType", param["ChangesType"]),
                                     new SqlParam("@OrderID", param["OID"]),
                                     new SqlParam("@Remark", param["Remark"]),
                                     new SqlParam("@Operator", param["Operator"]),
                                     new SqlParam("@OperatorType", param["OperatorType"]),
                                     new SqlParam("@OperatorWay",param["OperatorWay"]),
                                     new SqlParam("@State", param["State"])};
            sqls[4] = balanceAdd;
            objs[4] = parmBalanceAdd;

            // 发送短消息

            StringBuilder msgAdd = new StringBuilder();
            msgAdd.Append("Insert into YR_Messages(");
            msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
            msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.Append("订单：").Append(param["OrderNum"].ToString()).Append(" 消费 ").Append(param["SettlementMoney"].ToString()).Append("元");
            SqlParam[] parmMsgAdd = new SqlParam[] { 
                                     new SqlParam("@ID", param["MessageID"]),
                                     new SqlParam("@OperatorID", param["Operator"]),
                                     new SqlParam("@UserID", param["UserID"]),
                                     new SqlParam("@MessageTitle", "订单消费通知"),
                                     new SqlParam("@MessageContent",sbMsg.ToString()),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
            sqls[5] = msgAdd;
            objs[5] = parmMsgAdd;

            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls, objs);
        }
    }
}
