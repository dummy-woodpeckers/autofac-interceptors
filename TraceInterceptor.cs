
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace autofac_interceptors
{
    public class TraceInterceptor : IInterceptor
    {
        private readonly ILoggerFactory _loggerFactory;

        public TraceInterceptor(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.GetCustomAttribute<TraceAttribute>() == null)
            {
                invocation.Proceed();
                return;
            }

            // todo: analyze Trace attribute
            var logger = _loggerFactory.CreateLogger(invocation.TargetType);
            using var scope = logger.BeginScope(invocation.Method);
            logger.LogInformation("* before");
            invocation.Proceed();
            logger.LogInformation("* after");
        }
    }
}
