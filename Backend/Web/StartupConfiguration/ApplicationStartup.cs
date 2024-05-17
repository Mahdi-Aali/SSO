namespace Web.StartupConfiguration;

public abstract class ApplicationStartup
{
    public virtual void ConfigureService(WebApplicationBuilder builder)
    {

    }

    public virtual async Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        await app.RunAsync();
    }
}
