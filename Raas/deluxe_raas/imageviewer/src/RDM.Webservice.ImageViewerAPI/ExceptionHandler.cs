using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using RDM.Webservice.ImageViewerAPI.Factories;
using Serilog;

namespace RDM.Webservice.ImageViewerAPI
{
    public class ExceptionHandler
    {
        public async Task Invoke(HttpContext context, IMonitorFactory monitorFactory)
        {
            var monitor = monitorFactory.Get();
            monitor.StopAll();
            context.Response.ContentType = "application/json";

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            switch (ex)
            {
                case InvalidDataException _:
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    Log.Logger.Information(ex, "Returns a 400 from imageviewer.");
                    await context.Response.WriteAsync(string.Empty);

                    break;
                }

                default:
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Log.Logger.Fatal(
                        ex,
                        "About to return a generic 500 from imageviewer. Please contact the deployment team about this issue.");
                    await context.Response.WriteAsync(@"{ ""Message"" : ""The server has failed"" }");

                    break;
                }
            }
        }
    }
}