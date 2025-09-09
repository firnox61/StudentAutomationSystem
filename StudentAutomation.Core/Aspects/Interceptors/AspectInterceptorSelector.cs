
using Castle.DynamicProxy;
using System.Reflection;

using StudentAutomation.Core.Aspects.Logging;
using StudentAutomation.Core.Aspects.Performance;

namespace StudentAutomation.Core.Aspects.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();

            // method base class'da mı? Eğer interface metodunu aldıysan, class'taki karşılığını bul
            var targetMethod = type.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());

            var methodAttributes = targetMethod != null
                ? targetMethod.GetCustomAttributes<MethodInterceptionBaseAttribute>(true)
                : Enumerable.Empty<MethodInterceptionBaseAttribute>();

            Console.WriteLine("🎯 Class Attributes:");
            foreach (var attr in classAttributes)
                Console.WriteLine($" - {attr.GetType().Name}");

            Console.WriteLine("🎯 Method Attributes:");
            foreach (var attr in methodAttributes)
                Console.WriteLine($" - {attr.GetType().Name}");

            classAttributes.AddRange(methodAttributes);

            classAttributes.Add(new LogAspect { Priority = 99 });
            classAttributes.Add(new PerformanceAspect(3) { Priority = 98 });
           /* if (method.Name != "Get") // örnek: sadece Get hariç hepsine log atalım
            {
                classAttributes.Add(new AuditLogAspect { Priority = 97 });
            }*/

            return classAttributes
                .OrderBy(x => x.Priority)
                .Cast<IInterceptor>()
                .ToArray();
        }

    }

}
