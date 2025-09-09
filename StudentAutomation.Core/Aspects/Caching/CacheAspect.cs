using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using StudentAutomation.Core.Utilities.IoC;
using StudentAutomation.Core.Caching;
using StudentAutomation.Core.Aspects.Interceptors;

namespace StudentAutomation.Core.Aspects.Caching
{
    public class CacheAspect : MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;

        public CacheAspect(int duration = 60)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";//?? varsa sola yoksa sağa ekle
            if (_cacheManager.IsAdd(key))
            {
                Console.WriteLine("🟢 Cache'ten veri döndü");
                invocation.ReturnValue = _cacheManager.Get(key);
                return;
            }
            invocation.Proceed();
            _cacheManager.Add(key, invocation.ReturnValue, _duration);
            Console.WriteLine("🔵 Cache'e yeni veri eklendi");
        }
    }
}
