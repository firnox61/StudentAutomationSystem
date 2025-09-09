using Castle.DynamicProxy;

namespace StudentAutomation.Core.Aspects.Interceptors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class MethodInterceptionBaseAttribute : Attribute, IInterceptor, IAsyncInterceptor
    {
        public int Priority { get; set; }

        public virtual void Intercept(IInvocation invocation) { }

        public virtual void InterceptSynchronous(IInvocation invocation)
        {
            Intercept(invocation);
        }

        public virtual void InterceptAsynchronous(IInvocation invocation)
        {
            InterceptAsyncInternal(invocation).GetAwaiter().GetResult();
        }

        public virtual void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            InterceptAsyncInternal(invocation).GetAwaiter().GetResult();
        }

        private async Task InterceptAsyncInternal(IInvocation invocation)
        {
            Intercept(invocation);
        }
    }

}
