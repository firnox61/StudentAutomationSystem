using Microsoft.Extensions.DependencyInjection;

namespace StudentAutomation.Core.Utilities.IoC
{
    public interface ICoreModule
    {
        void Load(IServiceCollection serviceCollection);
    }
}
