using Infrastructure;
using System.Reflection;
using Web.StartupConfiguration.ConfigurationLoaderExtenssion;
using Web.StartupConfiguration.ServiceConfigurationLoadersExtenssion;



namespace Web.StartupConfiguration;

public abstract class WebApplicationStartup : ApplicationStartup
{
    public IConfiguration Configuration { get; private set; }
    private Assembly[] _assemblies;


    protected WebApplicationStartup()
    {
        LoadStartupDependencies();
    }


    public override void ConfigureService(WebApplicationBuilder builder)
    {
        base.ConfigureService(builder);

        builder.Services
            .LoadInfrastrutureDependencies(Configuration)
            .LoadServiceConfigurations(Configuration);
    }

    public override async Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.LoadApplicationConfigurations(env);
        await base.Configure(app, env);
    }






    private void LoadApplicationConfigurations()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }

    private void LoadAssemblies()
    {
        _assemblies =
            Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies()
            .Where(asm => asm.Name!.Contains("sso", StringComparison.OrdinalIgnoreCase))
            .Select(asm => Assembly.Load(asm))
            .ToArray();
    }

    private void LoadStartupDependencies()
    {
        LoadApplicationConfigurations();
        LoadAssemblies();
    }
}
