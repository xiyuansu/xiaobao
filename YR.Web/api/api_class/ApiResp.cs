using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.api_class
{
    public class ApiResp
    {
        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

    }
}