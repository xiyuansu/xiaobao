using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiasofti.SmartVehicle.Common;
using YR.Busines;
using YR.Common;
using YR.DataBase;
using System.Data;
using System.Web;
using System.Collections;
using Asiasofti.SmartVehicle.Common.Enum;
using YR.Common.DotNetCode;
using System.Security.Cryptography;
using YR.Common.DotNetSMS;
using YR.Common.DotNetConfig;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 短信管理
    /// </summary>
    public class SmsInfoManager
    {
        private int VerCodeValidTime = 10;

        public SmsInfoManager()
        {
            //初始化验证码有效时间
            int.TryParse(SiteHelper.GetAppsetString("VerificationCodeValidTime"), out VerCodeValidTime);
        }

        /// <summary>
        /// 根据手机获取最近xx分钟之内发的最后一条验证码(用于登录验证码判断)
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>验证码，验证码为空表示验证码已经过期</returns>
        public string GetVerCodeByMobile(string mobile)
        {
            StringBuilder strSql = new StringBuilder();
            string time = SiteHelper.GetWebServerCurrentTime().AddMinutes(-VerCodeValidTime).ToString();
            strSql.Append(@"select top 1 ShortMessage from yr_smslog where sendtime>='" + time + "' and MessageState='" + (int)SMSMessageState.AlreadySend + "' and Receiver='" + mobile + "'  order by sendtime desc");
            return DataFactory.SqlDataBase().GetSingleValueBySQL(strSql);
        }

        /// <summary>
        /// 获取指定手机号特定日期发送的短信数量
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetSMSCount(string mobile, DateTime dt)
        {
            StringBuilder strSql = new StringBuilder();
            string dtstr = DateTime.Now.ToString("yyyyMMdd");
            strSql.Append(@"select count(1) ShortMessage from yr_smslog where sendtime>='" + dtstr + "' and sendtime<='" + dtstr + " 23:59:59' and MessageState='" + (int)SMSMessageState.AlreadySend + "' and Receiver='" + mobile + "'");
            return int.Parse(DataFactory.SqlDataBase().GetSingleValueBySQL(strSql));
        }

        public int AddSMSLog(Hashtable log)
        {
            log["SendTime"] = SiteHelper.GetWebServerCurrentTime().ToString();

            return DataFactory.SqlDataBase().InsertByHashtable("YR_SMSLog", log);
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool SendVerificationCode(string mobile)
        {
            string code = SiteHelper.CreateRandom(true, 4);
            bool result = SMSFactory.GetSMS().SendCheckCode(mobile, code);
            Hashtable hashLog = new Hashtable();
            hashLog["ID"] = CommonHelper.GetGuid;
            hashLog["Sender"] = mobile;
            hashLog["Receiver"] = mobile;
            hashLog["ShortMessage"] = code;
            hashLog["AllMessage"] = string.Format("尊敬的用户，您的验证码是{0}。请不要把验证码泄露给其他人。如非本人操作，请忽略本短信", code);
            hashLog["MessageType"] = (int)SMSMessageType.User;
            if (result)
            {
                hashLog["MessageState"] = (int)SMSMessageState.AlreadySend;
            }
            else
            {
                hashLog["MessageState"] = (int)SMSMessageState.SendFaild;
            }
            AddSMSLog(hashLog);
            return result;
        }

        public bool SendAlarm(string mobile, string vehicleGPSNum, string vehicleName, string type)
        {
            string company = ConfigHelper.GetAppSettings("SMS_CompanyName");
            string message=string.Format("【{0}】车辆{1}，{2}发生{3}报警，请尽快排查。", company, vehicleName, vehicleGPSNum, type);
            bool result = new ZTSMS().SendAlarmMessage(mobile, message);
            Hashtable hashLog = new Hashtable();
            hashLog["ID"] = CommonHelper.GetGuid;
            hashLog["Sender"] = mobile;
            hashLog["Receiver"] = mobile;
            hashLog["ShortMessage"] = type;
            hashLog["AllMessage"] = message;
            hashLog["MessageType"] = (int)SMSMessageType.User;
            if (result)
            {
                hashLog["MessageState"] = (int)SMSMessageState.AlreadySend;
            }
            else
            {
                hashLog["MessageState"] = (int)SMSMessageState.SendFaild;
            }
            AddSMSLog(hashLog);
            return result;
        }

        public bool SendReturn(string mobile)
        {
            string company = ConfigHelper.GetAppSettings("SMS_CompanyName");
            string message=string.Format("【{0}】{1}", company, "尊敬的客户，您当前有未还车订单，避免造成额外的费用，小宝出行提醒您及时还车，谢谢您对小宝出行的支持！");
            bool result = new ZTSMS().SendAlarmMessage(mobile, message);
            Hashtable hashLog = new Hashtable();
            hashLog["ID"] = CommonHelper.GetGuid;
            hashLog["Sender"] = mobile;
            hashLog["Receiver"] = mobile;
            hashLog["ShortMessage"] = "还车提醒";
            hashLog["AllMessage"] = message;
            hashLog["MessageType"] = (int)SMSMessageType.User;
            if (result)
            {
                hashLog["MessageState"] = (int)SMSMessageState.AlreadySend;
            }
            else
            {
                hashLog["MessageState"] = (int)SMSMessageState.SendFaild;
            }
            AddSMSLog(hashLog);
            return result;
        }

        /// <summary>
        /// 短信列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetSmsInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_SMSLog where 1=1");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "SendTime", "Desc", pageIndex, pageSize, ref count);
        }

    }
}
