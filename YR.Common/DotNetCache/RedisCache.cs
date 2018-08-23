using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YR.Common.DotNetConfig;

namespace YR.Common.DotNetCache
{
    public class RedisCache:ICache
    {
        private ConnectionMultiplexer redis=null;

        private IDatabase db=null;

        public RedisCache()
        {
            string configuration = ConfigHelper.GetAppSettings("redis_db");
            //redis = ConnectionMultiplexer.Connect("101.200.49.244:6379,ssl=false,password=redis#mengshi@2017");
            redis = ConnectionMultiplexer.Connect(configuration);
            db = redis.GetDatabase();
        }

        public void Set(string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            db.StringSet(key, json);
        }

        public void Set(string key, object value,TimeSpan ts)
        {
            string json = JsonConvert.SerializeObject(value);
            db.StringSet(key, json, ts);
        }

        public T Get<T>(string key)
        {
            string json= db.StringGet(key);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return default(T);
            }
        }

        public void Remove(string key)
        {
            db.KeyDelete(key);
        }

        public void Dispose()
        {
            if(redis!=null && redis.IsConnected)
            {
                redis.Close();
            }
            redis = null;
            db = null;
        }
    }

}
