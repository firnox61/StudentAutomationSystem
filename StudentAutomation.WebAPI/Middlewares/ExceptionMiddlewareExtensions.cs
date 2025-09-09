namespace StudentAutomation.WebAPI.Middlewares
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();//yaşam döngüsünde çalıştırlmak istenen kod nedir ExceptionMiddleware()
        }
    }
}
