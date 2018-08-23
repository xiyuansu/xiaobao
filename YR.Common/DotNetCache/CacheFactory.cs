using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetCache
{
    public class CacheFactory
    {
        public static ICache GetCache()
        {
            return new RedisCache();
        }
    }
}
