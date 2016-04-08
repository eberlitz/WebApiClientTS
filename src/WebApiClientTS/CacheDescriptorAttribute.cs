using System;

namespace WebApiClientTS
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheDescriptorAttribute : Attribute
    {
        public CacheDescriptorAttribute()
        {
            this.Cache = true;
        }

        public bool Cache { get; set; }
    }
}