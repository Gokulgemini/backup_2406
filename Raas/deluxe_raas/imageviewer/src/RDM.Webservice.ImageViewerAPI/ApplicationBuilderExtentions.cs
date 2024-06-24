using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RDM.Services.ImageViewerAPI;

namespace RDM.Webservice.ImageViewerAPI
{
    public static class ApplicationBuilderExtentions
    {
        public static OperationsService Service { get; set; }

        public static IApplicationBuilder UseOperationsService(this IApplicationBuilder app)
        {
            Service = app.ApplicationServices.GetService<OperationsService>();

            var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            Service.Start();
        }

        private static void OnStopping()
        {
            Service.Stop();
        }
    }
}
