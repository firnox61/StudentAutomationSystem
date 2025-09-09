using Castle.DynamicProxy;
namespace StudentAutomation.Core.Aspects.Interceptors
{
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { }
        protected virtual void OnAfter(IInvocation invocation) { }
        protected virtual void OnException(IInvocation invocation, Exception e) { }
        protected virtual void OnSuccess(IInvocation invocation) { }

        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                isSuccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSuccess) OnSuccess(invocation);
                OnAfter(invocation);
            }
        }

        public override void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsyncInternal(invocation);
        }

        public override void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsyncInternal(invocation);
        }

        private async Task InterceptAsyncInternal(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                await invocation.ProceedAsync();
            }
            catch (Exception e)
            {
                isSuccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSuccess) OnSuccess(invocation);
                OnAfter(invocation);
            }
        }
    }
}
