using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyInjection;

public interface IDependencyInjectionProvider
{
    public abstract static IServiceCollection GetAssemblyDependencies(IServiceCollection services, IConfiguration configuration);
}
