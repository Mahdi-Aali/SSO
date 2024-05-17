using Quartz;
using Web.Modules.QuartzScheduler;

namespace Web.StartupConfiguration.ConfigurationLoaderExtenssion;

public static class WebApplicationConfigurationLoaderExtenssion
{

    public static async void LoadApplicationConfigurations(this WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        if (env.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapGet("/hc", async (HttpContext context) =>
        {
            await context.Response.WriteAsync("Healthy...");
        });

        await ScheduleJobs(app);
    }


    private static async Task ScheduleJobs(WebApplication app)
    {
        ISchedulerFactory schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        IScheduler scheduler = await schedulerFactory.GetScheduler();


        await scheduler.ScheduleJobs(QuartzSchedulerFactory.GetJobsAndTriggers(), false);

        await Task.CompletedTask;
    }
}