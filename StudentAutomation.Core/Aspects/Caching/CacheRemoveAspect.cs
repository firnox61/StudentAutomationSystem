
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using StudentAutomation.Core.Aspects.Interceptors;
using StudentAutomation.Core.Caching;
using StudentAutomation.Core.Utilities.IoC;

namespace StudentAutomation.Core.Aspects.Caching
{
    public class CacheRemoveAspect : MethodInterception
    //datada olan güncelleme yeni ekleme vs durumlarında cachein silinmesi gerekir
    {
        private string _pattern;
        private ICacheManager _cacheManager;

        public CacheRemoveAspect(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            _cacheManager.RemoveByPattern(_pattern);
            Console.WriteLine($"❌ Cache silindi: pattern = {_pattern}");
        }
    }
}
