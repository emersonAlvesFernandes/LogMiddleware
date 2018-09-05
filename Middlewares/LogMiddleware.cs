using CustomMiddleware.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomMiddleware.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogRepository _logRepository;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                this._logRepository = (ILogRepository)context.RequestServices.GetService(typeof(ILogRepository));

                await _next(context);

                LogSuccess(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }


        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 

            //InvalidOperationException : pode utilizar exceções customizadas
            if (exception is InvalidOperationException) code = HttpStatusCode.BadRequest;

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            LogErrorAsync(context, exception);

            return context.Response.WriteAsync(result);
        }

        private void LogSuccess(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            var responseBody = this.GetResponse(context);

            var requestBodySerialized = this.GetRequest(context);

            var logobj = new ApplicationLog(
                context.Request.Host.ToString(),
                context.Request.ContentType,
                context.Request.Path,
                context.Request.Method,
                requestBodySerialized,
                "false",
                string.Empty,
                string.Empty,
                200,
                responseBody);

            _logRepository.Log(logobj);

        }

        private void LogErrorAsync(HttpContext context, Exception ex)
        {

            Stream originalBody = context.Response.Body;

            #region read response

            string responseBody = string.Empty;
            using (var memStream = new MemoryStream())
            {
                context.Response.Body = memStream;

                memStream.Position = 0;
                responseBody = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                memStream.CopyToAsync(originalBody);
            }

            #endregion

            #region read request

            var bodyStr = string.Empty;
            var req = context.Request;
            req.EnableRewind();
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            { bodyStr = reader.ReadToEnd(); }
            req.Body.Position = 0;

            var req2 = context.Request;
            req2.EnableRewind();
            using (StreamReader reader = new StreamReader(req2.Body, Encoding.UTF8, true, 1024, true))
            { bodyStr = reader.ReadToEnd(); }
            req2.Body.Position = 0;
            var requestBody2 = JsonConvert.DeserializeObject<object>(bodyStr);
            var requestBodySerialized = JsonConvert.SerializeObject(bodyStr);

            var requestBody = JsonConvert.DeserializeObject<object>(bodyStr);

            #endregion

            var logobj = new ApplicationLog(
                context.Request.Host.ToString(),
                context.Request.ContentType,
                context.Request.Path,
                context.Request.Method,
                requestBodySerialized,
                "true",
                ex.Message,
                ex.StackTrace,
                context.Response.StatusCode,
                responseBody);

            this._logRepository.Log(logobj);
        }

        private string GetRequest(HttpContext context)
        {
            var bodyStr = string.Empty;

            var req = context.Request;
            req.EnableRewind();
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true)) { bodyStr = reader.ReadToEnd(); }
            req.Body.Position = 0;
            var requestBody = JsonConvert.DeserializeObject<object>(bodyStr);

            var req2 = context.Request;
            req2.EnableRewind();
            using (StreamReader reader = new StreamReader(req2.Body, Encoding.UTF8, true, 1024, true))
            { bodyStr = reader.ReadToEnd(); }
            req2.Body.Position = 0;
            var requestBody2 = JsonConvert.DeserializeObject<object>(bodyStr);
            var requestBodySerialized = JsonConvert.SerializeObject(bodyStr);

            return requestBodySerialized;
        }

        private string GetResponse(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            string responseBody = string.Empty;

            using (var memStream = new MemoryStream())
            {
                context.Response.Body = memStream;

                memStream.Position = 0;
                responseBody = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                memStream.CopyToAsync(originalBody);

                return responseBody;
            }
        }
    }
}
