using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.api_class
{
    public class ObjectApiResp:ApiResp
    {
        private object data;

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

    }
}