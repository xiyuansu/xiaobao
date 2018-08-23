using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetSMS
{
    public interface ISMS
    {
        bool SendMessage(string telphone, string message);

        bool SendCheckCode(string telphone, string code);
    }
}
