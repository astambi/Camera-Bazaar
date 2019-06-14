namespace CameraBazaar.Web.Infrastructure.FIlters
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class LogAttribute : ActionFilterAttribute
    {
        private const string LogsFileName = "logs.txt";
        private const string AnonymousUser = "Anonymous";

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Task.Run(async () =>
            {
                var dateTime = DateTime.UtcNow;
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
                var user = context.HttpContext.User?.Identity?.Name ?? AnonymousUser;
                var controller = context.Controller.GetType().Name;
                var action = context.RouteData.Values["action"].ToString();

                var logMessage = $"{dateTime} - {ipAddress} - {user} - {controller}.{action}";

                if (context.Exception != null)
                {
                    var exceptionType = context.Exception.GetType().Name;
                    var exceptionMessage = context.Exception.Message;

                    logMessage = $"[!] {logMessage} - {exceptionType} - {exceptionMessage}";
                }

                using (var writer = new StreamWriter(LogsFileName, true)) // append
                {
                    await writer.WriteLineAsync(logMessage);
                }
            })
            .GetAwaiter()
            .GetResult();
        }
    }
}
