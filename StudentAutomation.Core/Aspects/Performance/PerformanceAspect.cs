using Castle.DynamicProxy;
using StudentAutomation.Core.Aspects.Interceptors;
using System.Diagnostics;
using Serilog;
namespace StudentAutomation.Core.Aspects.Performance
{
    public class PerformanceAspect : MethodInterception
    {
        private readonly int _interval;
        private readonly Stopwatch _stopwatch = new();

        public PerformanceAspect(int interval = 3) // default 3s
        {
            _interval = interval;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Restart();
        }

        protected override void OnAfter(IInvocation invocation)
        {
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.TotalSeconds > _interval)
            {
                var method = $"{invocation.Method.DeclaringType!.FullName}.{invocation.Method.Name}";
                Log.Warning("PERFORMANCE WARNING: {Method} took {Seconds:F2} seconds.",
                            method, _stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
