namespace Web.StartupConfiguration;

public abstract class Startup<TStartup> where TStartup : ApplicationStartup
{
    protected static async Task RunAsync(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        TStartup startUp = (TStartup)Activator.CreateInstance(typeof(TStartup))!;

        startUp.ConfigureService(builder);
        await startUp.Configure(builder.Build(), builder.Environment);
    }
}
