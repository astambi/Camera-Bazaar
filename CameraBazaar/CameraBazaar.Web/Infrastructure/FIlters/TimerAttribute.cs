namespace CameraBazaar.Web.Infrastructure.FIlters
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class TimerAttribute : ActionFilterAttribute
    {
        private const string TimerLogsFileName = "action-times.txt";

        private Stopwatch stopwatch;

        public override void OnActionExecuting(ActionExecutingContext context)
            => this.stopwatch = Stopwatch.StartNew();

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Task.Run(async () =>
            {
                this.stopwatch.Stop();

                var dateTime = DateTime.UtcNow;
                var controller = context.Controller.GetType().Name;
                var action = context.RouteData.Values["action"].ToString();
                var elapsedTime = this.stopwatch.Elapsed;

                var logMessage = $"{dateTime} - {controller}.{action} - {elapsedTime}";

                using (var writer = new StreamWriter(TimerLogsFileName, true))
                {
                    await writer.WriteLineAsync(logMessage);
                }
            })
            .GetAwaiter()
            .GetResult();
        }
    }
}