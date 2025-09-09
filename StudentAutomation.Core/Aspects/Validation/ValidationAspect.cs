using Castle.DynamicProxy;
using FluentValidation;
using FluentValidation.Results;
using StudentAutomation.Core.Aspects.Interceptors;

namespace StudentAutomation.Core.Aspects.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ValidationAspect : MethodInterception
    {
        
        private readonly Type _validatorType;

        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new Exception("Bu bir doğrulama sınıfı değildir");
            }

            _validatorType = validatorType;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            Console.WriteLine("🚨 ValidationAspect çalıştı!");

            var validatorInstance = (IValidator)Activator.CreateInstance(_validatorType)!;
            var entityType = _validatorType.BaseType!.GetGenericArguments()[0];

            var entities = invocation.Arguments
                .Where(arg => arg != null && arg.GetType() == entityType);

            foreach (var entity in entities)
            {
                var context = new ValidationContext<object>(entity);
                var result = validatorInstance.Validate(context);

                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
            }
        }



    }

}
