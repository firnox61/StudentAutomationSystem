
using Microsoft.Extensions.DependencyInjection;

namespace StudentAutomation.Core.Utilities.IoC
{
    public static class ServiceTool
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        //bizim injectionlar yazabilmememizi sağlıyor
        public static IServiceCollection Create(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            return services;
        }
    }
}
