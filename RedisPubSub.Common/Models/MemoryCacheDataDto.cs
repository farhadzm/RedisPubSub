using System;
using System.Collections.Generic;
using System.Text;

namespace RedisPubSub.Common.Models
{
    public class MemoryCacheDataDto
    {
        public string CacheKey { get; set; }
        public object Data { get; set; }
    }
}
