using Castle.DynamicProxy;
using StudentAutomation.Core.Aspects.Interceptors;
using Serilog;

namespace StudentAutomation.Core.Aspects.Logging
{
    public class LogAspect : MethodInterception
    {
        protected override void OnBefore(IInvocation invocation)
        {
            var methodName = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";
            var arguments = string.Join(", ", invocation.Arguments.Select(a => a?.ToString() ?? "null"));
            Log.Information("[BEFORE] {Method} args: {Args}", methodName, arguments);
        }

        protected override void OnException(IInvocation invocation, Exception e)
        {
            var methodName = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";
            Log.Error(e, "[EXCEPTION] {Method}", methodName);
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            var methodName = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";
            Log.Information("[SUCCESS] {Method}", methodName);
        }
    }
}
