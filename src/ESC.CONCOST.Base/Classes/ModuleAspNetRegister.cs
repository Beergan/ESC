using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Base;

public class ModuleAspNetRegister : IModuleAspNet
{
    public void BuildModule(IApplicationBuilder app)
    {
    }

    public void ConfigureServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        services.AddScoped<IServiceBase, MyServiceBase>();
    }
}