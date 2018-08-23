using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Common.DotNetCache
{
    public interface ICache:IDisposable
    {
        void Set(string key,object value);

        void Set(string key, object value,TimeSpan t);

        T Get<T>(string key);

        void Remove(string key);
    }
}
