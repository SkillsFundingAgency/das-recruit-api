using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Recruit.Api.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Recruit.Api.Extensions;
using Microsoft.AspNetCore.Diagnostics;

namespace SFA.DAS.Recruit.Api
{
    public partial class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            var instance = HostingHelper.GetWebsiteInstanceId();

            applicationLifetime.ApplicationStarted.Register(() => logger.LogInformation($"Host fully started: ({instance})"));
            applicationLifetime.ApplicationStopping.Register(() => logger.LogInformation($"Host shutting down...waiting to complete requests: ({instance})"));
            applicationLifetime.ApplicationStopped.Register(() => logger.LogInformation($"Host fully stopped. All requests processed: ({instance})"));

            if (env.IsDevelopment())
            {
                var configuration = (TelemetryConfiguration)app.ApplicationServices.GetService(typeof(TelemetryConfiguration));
                configuration.DisableTelemetry = true;
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseAuthentication();
            }

            app.ConfigureExceptionHandler(app.ApplicationServices.GetService<ILogger<ExceptionHandlerFeature>>());

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}