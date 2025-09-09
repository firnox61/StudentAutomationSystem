using Castle.DynamicProxy;

namespace StudentAutomation.Core.Aspects.Interceptors
{
    public static class InvocationExtensions
    {
        public static async Task ProceedAsync(this IInvocation invocation)
        {
            invocation.Proceed();
            var returnValue = invocation.ReturnValue;

            if (returnValue is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }

        public static async Task<TResult> ProceedAsync<TResult>(this IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            return await task.ConfigureAwait(false);
        }
    }

}