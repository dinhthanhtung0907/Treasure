using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Treasure.Helpers.Middlewares
{
    public class ErrorHandlerMiddleware

    {
        private readonly RequestDelegate _next;
        private IDistributedCache _cache;

        public ErrorHandlerMiddleware(
            IDistributedCache cache,
            RequestDelegate next)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                //Set response ContentType
                response.ContentType = "application/json";

                //Set custome error message for response model
                string[] messages = error.Message.Split('|');
                ResponseContent responseContent;
                if (int.TryParse(messages[0], out int error_code))
                {
                    responseContent = new ResponseContent()
                    {
                        ErrorCode = int.Parse(messages[0]),
                        Message = messages[1],
                        Path = "/"
                    };
                    //handler many Exception types
                    response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    responseContent = new ResponseContent
                    {
                        ErrorCode = 7777,
                        Message = error.StackTrace + "--|--" + error.InnerException + "--|--" + error.Message,
                        Path = "/system-error/debug-only"
                    };
                    //handler many Exception types
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                }
                //Using Newtonsoft.Json to convert object to json string
                var jsonResult = JsonSerializer.Serialize(responseContent);
                await response.WriteAsync(jsonResult);
            }
        }

        //Response Model
        public class ResponseContent
        {
            [JsonPropertyName("error_code")]
            public int ErrorCode { get; set; }
            [JsonPropertyName("msg")]
            public string Message { get; set; } = string.Empty;
            [JsonPropertyName("timestamp")]
            public string Timestamp { get; set; } = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString();
            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;

        }
    }
}
