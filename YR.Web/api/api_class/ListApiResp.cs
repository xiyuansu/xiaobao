using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.api_class
{
    public class ListApiResp:ApiResp
    {
        private int total;

        public int Total
        {
            get { return total; }
            set { total = value; }
        }

        private IList list;

        public IList List
        {
            get { return list; }
            set { list = value; }
        }


    }
}