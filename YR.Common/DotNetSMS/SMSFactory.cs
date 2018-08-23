using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetSMS
{
    public class SMSFactory
    {
        public static ISMS GetSMS()
        {
            return new ZTSMS();
        }
    }
}
