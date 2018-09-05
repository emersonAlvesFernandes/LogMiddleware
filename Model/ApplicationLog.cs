using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleware.Model
{
    public class ApplicationLog
    {
        public ApplicationLog(
            string host,
            string contentType,
            string endpoint,
            string method,
            object body,
            string exceptionOccurred,
            string messageException,
            string stackTrace,
            int statusCodeResponse,
            object bodyResponse)
        {
            Host = host;
            ContentType = contentType;
            Endpoint = endpoint;
            Method = method;
            Body = body;
            ExceptionOccurred = exceptionOccurred;
            MessageException = messageException;
            StackTrace = stackTrace;
            StatusCodeResponse = statusCodeResponse;
            BodyResponse = bodyResponse;
            DateOccurred = DateTime.Now;
            RequestId = new Guid().ToString();
        }

        public string Host { get; set; }
        public string ContentType { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public object Body { get; set; }
        public string ExceptionOccurred { get; set; }
        public string MessageException { get; set; }
        public string StackTrace { get; set; }
        public int StatusCodeResponse { get; set; }
        public object BodyResponse { get; set; }
        public DateTime DateOccurred { get; set; }
        public string RequestId { get; set; }        
    }
}
