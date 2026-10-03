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

                // Take an Action With the Response

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, " Something went Wrong  ");

                // Change Status Code
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // Change Content Type
                httpContext.Response.ContentType = "application/json";

                // Write Response Type
                var response = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
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
