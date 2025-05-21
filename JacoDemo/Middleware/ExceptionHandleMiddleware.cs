namespace JacoDemo.Middleware
{
    public class ExceptionHandleMiddleware
    {
        private RequestDelegate _next;

        public ExceptionHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //Save exception to log
            exception.ToString();

            //Customize Error Response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsync("Oops !! 系統發生錯誤，請稍後再試。");
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
    }
}
