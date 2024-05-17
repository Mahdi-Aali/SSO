using Web.StartupConfiguration;

public class Program : Startup<SSOStartup>
{
    static async Task Main(string[] args)
    {
        await RunAsync(args);
    }
}

public class SSOStartup : WebApplicationStartup
{

}