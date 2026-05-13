using System.Linq;
using System.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleSettingCore;

namespace ESC.CONCOST.ModuleSetting;

public class ModuleAspNetRegister : IModuleAspNet
{
    public void BuildModule(IApplicationBuilder app)
    {
        GlobalPermissions.Register(typeof(PERMISSION));
    }

    public void ConfigureServices(IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration config)
    {
    }
}