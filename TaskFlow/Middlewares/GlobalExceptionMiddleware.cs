using Domain.Exceptions;
using Shared.Errors;
using System.Text.Json;

namespace TaskFlow.Web.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Take an Action With the Request

                await _next.Invoke(httpContext); // Go to the Next Middleware 

                //catch مش موجود غير كدا هيخش ال Url فقط يعني لو طلب response هيخش هنا لو فيه مشكله ف
                // Take an Action With the Response
                if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    httpContext.Response.ContentType = "application/json";
                    var response = new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"The Endpoint With Url:{httpContext.Request.Path} Not Found"
                    };
                    await httpContext.Response.WriteAsJsonAsync(response);
                }

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, " Something went Wrong  ");

                // Change Status Code
                httpContext.Response.StatusCode = ex switch
                {
                    NotFoundExceptions => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                };
                // Change Content Type
                httpContext.Response.ContentType = "application/json";

                // Write Response Type
                var response = new ErrorDetails()
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = ex.Message
                };

                // capetal وبعدين اول حرف من الكلمه التانيه small هيتعمل او حرف  ErrorRespone عشان الفرونت يفهم ال
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                await httpContext.Response.WriteAsJsonAsync(response, options);
            }
        }
    }
}
