using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisPubSub.Models
{
    public class MemoryCacheDataDto
    {
        public string CacheKey { get; set; }
        public object Data { get; set; }
    }
}
