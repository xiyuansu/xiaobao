using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace YR.Web.App_Code
{
    public class MyWorkerRequest : SimpleWorkerRequest
    {
        private string localAdd = string.Empty;

        public MyWorkerRequest(string page, string query, TextWriter output, string address)
            : base(page, query, output)
        {
            this.localAdd = address;
        }

        public override string GetLocalAddress()
        {
            return this.localAdd;
        }

        public static HttpContext CreateHttpContext(string host,string page,string query)
        {
            TextWriter tw = new StringWriter();
            HttpWorkerRequest wr = new MyWorkerRequest(page, query, tw, host);
            HttpContext.Current = new HttpContext(wr);
            return HttpContext.Current;
        }
    }
}