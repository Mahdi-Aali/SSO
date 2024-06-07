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
            .LoadServiceConfigurations(Configuration, _assemblies);
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
        string[] validAssemblies = ["Domain", "Web", "Infrastructure", "Core", "Application"];
        _assemblies =
            Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies()
            .Select(s => Assembly.Load(s))
            .Where(asm => validAssemblies.Contains(asm.GetName().Name))
            .Concat([GetType().Assembly])
            .ToArray();
    }

    private void LoadStartupDependencies()
    {
        LoadApplicationConfigurations();
        LoadAssemblies();
    }
}
