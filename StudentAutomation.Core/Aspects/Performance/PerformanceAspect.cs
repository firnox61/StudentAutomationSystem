using Castle.DynamicProxy;
using StudentAutomation.Core.Aspects.Interceptors;
using System.Diagnostics;

namespace StudentAutomation.Core.Aspects.Performance
{
    public class PerformanceAspect : MethodInterception
    {
        private readonly int _interval;
        private Stopwatch _stopwatch;

        public PerformanceAspect(int interval = 3) // default 3 saniye
        {
            _interval = interval;
            _stopwatch = new Stopwatch();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        protected override void OnAfter(IInvocation invocation)
        {
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.TotalSeconds > _interval)
            {
                var method = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";
                var message = $"PERFORMANCE WARNING: {method} took {_stopwatch.Elapsed.TotalSeconds:F2} seconds.";
                File.AppendAllText("logs/performance.txt", $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
            }


            /*_stopwatch.Stop();
    var method = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}";
    var duration = _stopwatch.Elapsed.TotalSeconds;

    var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    if (!Directory.Exists(logDir))
        Directory.CreateDirectory(logDir);

    var logPath = Path.Combine(logDir, "performance.txt");

    File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss} - {method} completed in {duration:F4} seconds.{Environment.NewLine}");*/
            //herzaman çalışşsın istersek
        }
    }
}
