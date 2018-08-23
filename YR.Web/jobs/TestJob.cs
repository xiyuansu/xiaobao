using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Manager;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using YR.Common.DotNetCode;
using YR.Common.DotNetLog;

namespace YR.Web.jobs
{
    public class TestJob : IJob
    {
        private static Log Logger = LogFactory.GetLogger(typeof(TestJob));

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("测试任务调用");
        }
    }
}