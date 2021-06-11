using System;

namespace autofac_interceptors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TraceAttribute : Attribute
    {
    }
}
